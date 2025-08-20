using System.Net.Http;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class StoresRequestedEventHandler : IEventHandler<StoresRequested>
    {
        private readonly ILogger<StoresRequestedEventHandler> _logger;
        private readonly IVarejOnlineApiService _apiService;
        private readonly IIntegrationService _integrationService;
        private readonly HttpClient _httpClient;
        private readonly SyncOutConfig _syncOutConfig;
        private readonly IEventDispatcher _dispatcher;

        public StoresRequestedEventHandler(
            ILogger<StoresRequestedEventHandler> logger,
            IVarejOnlineApiService apiService,
            IIntegrationService integrationService,
            IOptions<SyncOutConfig> syncOutConfig,
            HttpClient httpClient,
            IEventDispatcher dispatcher)
        {
            _logger = logger;
            _apiService = apiService;
            _integrationService = integrationService;
            _syncOutConfig = syncOutConfig.Value;
            _httpClient = httpClient;
            _dispatcher = dispatcher;
        }

        public Task HandleAsync(StoresRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stores requested for hub {HubKey}", @event.HubKey);
            return Task.CompletedTask;
        }
    }
}
