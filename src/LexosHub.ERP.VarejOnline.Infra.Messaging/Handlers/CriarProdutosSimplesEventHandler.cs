using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class CriarProdutosSimplesEventHandler : IEventHandler<CriarProdutosSimples>
    {
        private readonly ILogger<CriarProdutosSimplesEventHandler> _logger;
        private readonly ISqsRepository _syncOutSqsRepository;
        private readonly ProdutoViewMapper _produtoViewMapper;
        private readonly CriarProdutosConfiguraveisEventHandler _configuraveisHandler;

        public CriarProdutosSimplesEventHandler(ILogger<CriarProdutosSimplesEventHandler> logger, ISqsRepository syncOutSqsRepository, IOptions<SyncOutConfig> syncOutSqsConfig, ProdutoViewMapper produtoViewMapper)
        {
            _logger = logger;
            _syncOutSqsRepository = syncOutSqsRepository;
            _produtoViewMapper = produtoViewMapper;
            var syncOutConfig = syncOutSqsConfig.Value;
            _syncOutSqsRepository.IniciarFila($"{syncOutConfig.SQSBaseUrl}{syncOutConfig.SQSAccessKeyId}/{syncOutConfig.SQSName}");
        }

        public Task HandleAsync(CriarProdutosSimples @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Pgina processada recebida. Hub: {HubKey}, Incio: {Start}, Quantidade: {PageSize}, Processados: {ProcessedCount}, Produtos: {ProductsCount}",
                @event.HubKey,
                @event.Start,
                @event.PageSize,
                @event.ProcessedCount,
                @event.Produtos?.Count ?? 0);

            if (@event is not null && @event.Produtos.Any())
            {
                var mapped = _produtoViewMapper.MapSimples(@event.Produtos);

                if (mapped.Any())
                {
                    var json = JsonConvert.SerializeObject(
                        mapped,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            ContractResolver = new IgnoreEmptyEnumerablesResolver()
                        });

                    var notificacao = new NotificacaoAtualizacaoModel()
                    {
                        Chave = @event.HubKey,
                        DataHora = DateTime.Now,
                        Json = json,
                        TipoProcesso = TipoProcessoAtualizacao.Produto,
                        PlataformaId = 41
                    };
                    _syncOutSqsRepository.AdicionarMensagemFilaFifo(notificacao, $"notificacao-syncout-{notificacao.Chave}");
                }
        }
        return Task.CompletedTask;
    }

    //public async Task HandleAsync(CriarProdutosConfiguraveis @event, CancellationToken cancellationToken)
    //    => await _configuraveisHandler.HandleAsync(@event, cancellationToken);
}
}
