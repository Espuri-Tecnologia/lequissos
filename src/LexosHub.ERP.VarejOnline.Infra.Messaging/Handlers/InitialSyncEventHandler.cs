using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class InitialSyncEventHandler : IEventHandler<InitialSync>
    {
        private readonly ILogger<InitialSyncEventHandler> _logger;
        private readonly IEventDispatcher _dispatcher;

        public InitialSyncEventHandler(ILogger<InitialSyncEventHandler> logger, IEventDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task HandleAsync(InitialSync @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initial sync started for hub {HubKey}", @event.HubKey);

            var companiesEvent = new CompaniesRequested { HubKey = @event.HubKey };
            await _dispatcher.DispatchAsync(companiesEvent, cancellationToken);
        }
    }
}
