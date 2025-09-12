using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class InvoicesRequestedEventHandler : IEventHandler<InvoicesRequested>
    {
        private readonly ILogger<InvoicesRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;

        public InvoicesRequestedEventHandler(
            ILogger<InvoicesRequestedEventHandler> logger,
            IIntegrationService integrationService)
        {
            _logger = logger;
            _integrationService = integrationService;
        }

        public async Task HandleAsync(InvoicesRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Invoices requested for hub {HubKey}, numero {Number}", @event.HubKey, @event.Number);

            await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
        }
    }
}
