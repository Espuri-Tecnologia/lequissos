using Lexos.SQS.Interface;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class PriceTablesRequestedEventHandler : IEventHandler<PriceTablesRequested>
    {
        private readonly ILogger<PriceTablesRequestedEventHandler> _logger;
        private readonly ISqsRepository _syncOutSqsRepository;
        private readonly IVarejoOnlineApiService _apiService;
        private readonly IIntegrationService _integrationService;
        private readonly IEventDispatcher _dispatcher;
        private readonly int _defaultPageSize;

        public PriceTablesRequestedEventHandler(
            ILogger<PriceTablesRequestedEventHandler> logger,
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncOutConfig> syncOutSqsConfig,
            IVarejoOnlineApiService apiService,
            IIntegrationService integrationService,
            IConfiguration configuration,
            IEventDispatcher dispatcher)
        {
            _logger = logger;
            _syncOutSqsRepository = syncOutSqsRepository;
            var syncOutConfig = syncOutSqsConfig.Value;
            _syncOutSqsRepository.IniciarFila($"{syncOutConfig.SQSBaseUrl}{syncOutConfig.SQSAccessKeyId}/{syncOutConfig.SQSName}");
            _apiService = apiService;
            _integrationService = integrationService;
            _defaultPageSize = configuration.GetValue<int>("VarejoOnlineApiSettings:DefaultPageSize");
            _dispatcher = dispatcher;
        }

        public async Task HandleAsync(PriceTablesRequested @event, CancellationToken cancellationToken)
        {
            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            _logger.LogInformation(
                "Página de tabelas de preço processada. Hub: {HubKey}, Início: {Start}, Quantidade: {PageSize}, Processados: {ProcessedCount}, Tabelas: {PriceTableCount}",
                @event.HubKey,
                @event.Start,
                @event.PageSize,
                @event.ProcessedCount,
                @event.PriceTables?.Count ?? 0);

            var start = 0;
            var pageSize = _defaultPageSize;

            int count = 0;
            do
            {

                var response = await _apiService.GetPriceTablesAsync(token, count, pageSize);
                var produtos = response.Result;
                count = produtos?.Count ?? 0;

                if (produtos == null || count == 0)
                {
                    _logger.LogInformation("Nenhum produto retornado. Encerrando processamento.");
                    break;
                }

                var pageEvent = new PriceTablesRequested
                {
                    HubKey = @event.HubKey,
                    Start = start,
                    PageSize = pageSize,
                    PriceTables = response.Result,
                    ProcessedCount = count
                };

                await _dispatcher.DispatchAsync(pageEvent, cancellationToken);

                start += pageSize;

            } while (count >= pageSize);
            return;
        }
    }
}
