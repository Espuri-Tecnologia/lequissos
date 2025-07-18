using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers.Produto;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class ProductsPageProcessedEventHandler : IEventHandler<ProductsPageProcessed>
    {
        private readonly ILogger<ProductsPageProcessedEventHandler> _logger;
        private readonly ISqsRepository _syncOutSqsRepository;

        public ProductsPageProcessedEventHandler(ILogger<ProductsPageProcessedEventHandler> logger, ISqsRepository syncOutSqsRepository, IOptions<SyncOutConfig> syncOutSqsConfig)
        {
            _logger = logger;
            _syncOutSqsRepository = syncOutSqsRepository;
            var syncOutConfig = syncOutSqsConfig.Value;
            _syncOutSqsRepository.IniciarFila($"{syncOutConfig.SQSBaseUrl}{syncOutConfig.SQSAccessKeyId}/{syncOutConfig.SQSName}");
        }

        public Task HandleAsync(ProductsPageProcessed @event, CancellationToken cancellationToken)
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
                var mapped = @event.Produtos.Map();

                if (mapped is not null)
                {
                    var notificacao = new NotificacaoAtualizacaoModel()
                    {
                        Chave = @event.HubKey,
                        DataHora = DateTime.Now,
                        Json = JsonConvert.SerializeObject(mapped),
                        TipoProcesso = TipoProcessoAtualizacao.Produto,
                        PlataformaId = 41
                    };
                    _syncOutSqsRepository.AdicionarMensagemFilaFifo(notificacao, $"notificacao-syncout-{notificacao.Chave}");
                }
            }
            return Task.CompletedTask;
        }
    }
}
