using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejOnline.Api.Jobs
{
    public class StockSyncJobService
    {
        public const string RecurringJobId = "StockSyncRecurringJob";

        private readonly IIntegrationRepository _integrationRepository;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ILogger<StockSyncJobService> _logger;

        public StockSyncJobService(
            IIntegrationRepository integrationRepository,
            IEventDispatcher eventPublisher,
            ILogger<StockSyncJobService> logger)
        {
            _integrationRepository = integrationRepository ?? throw new ArgumentNullException(nameof(integrationRepository));
            _eventDispatcher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stock sync job started.");

            IEnumerable<IntegrationDto> integrations;

            try
            {
                integrations = await _integrationRepository.GetActiveIntegrations().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve active integrations for stock sync job.");
                throw;
            }

            foreach (var integration in integrations ?? Enumerable.Empty<IntegrationDto>())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (integration is null)
                {
                    _logger.LogWarning("Skipping null integration while running stock sync job.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(integration.HubKey) || string.IsNullOrWhiteSpace(integration.Token))
                {
                    _logger.LogWarning(
                        "Skipping integration {IntegrationId} due to missing hub key or token.",
                        integration.Id);
                    continue;
                }

                var stockRequest = new StocksRequested
                {
                    HubKey = integration.HubKey!,
                    Entidades = integration.Settings!.WarehouseBranchId
                };

                try
                {
                    await _eventDispatcher.DispatchAsync(stockRequest, cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation(
                        "StocksRequested event dispatched for integration {IntegrationId} (HubKey: {HubKey}).",
                        integration.Id,
                        integration.HubKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to dispatch StocksRequested event for integration {IntegrationId} (HubKey: {HubKey}).",
                        integration.Id,
                        integration.HubKey);
                }
            }

            _logger.LogInformation("Stock sync job finished.");
        }
    }
}