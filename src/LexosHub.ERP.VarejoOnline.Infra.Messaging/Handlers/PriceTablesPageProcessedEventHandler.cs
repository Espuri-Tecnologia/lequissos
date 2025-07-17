using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers.Preco;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class PriceTablesPageProcessedEventHandler : IEventHandler<PriceTablesPageProcessed>
    {
        private readonly ILogger<PriceTablesPageProcessedEventHandler> _logger;
        private readonly ISqsRepository _syncOutSqsRepository;

        public PriceTablesPageProcessedEventHandler(
            ILogger<PriceTablesPageProcessedEventHandler> logger,
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncOutConfig> syncOutSqsConfig)
        {
            _logger = logger;
            _syncOutSqsRepository = syncOutSqsRepository;
            var syncOutConfig = syncOutSqsConfig.Value;
            _syncOutSqsRepository.IniciarFila($"{syncOutConfig.SQSBaseUrl}{syncOutConfig.SQSAccessKeyId}/{syncOutConfig.SQSName}");
        }

        public Task HandleAsync(PriceTablesPageProcessed @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Página de tabelas de preço processada. Hub: {HubKey}, Início: {Start}, Quantidade: {PageSize}, Processados: {ProcessedCount}, Tabelas: {PriceTableCount}",
                @event.HubKey,
                @event.Start,
                @event.PageSize,
                @event.ProcessedCount,
                @event.PriceTables?.Count ?? 0);

            if (@event != null && @event.PriceTables.Any())
            {
                var mapped = @event.PriceTables.Map();

                if (mapped != null)
                {
                    var notificacao = new NotificacaoAtualizacaoModel
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
