using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class StoresRequestedEventHandler : IEventHandler<StoresRequested>
    {
        private readonly ILogger<StoresRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly SyncOutConfig _syncOutConfig;

        public StoresRequestedEventHandler(
            ILogger<StoresRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            IOptions<SyncOutConfig> syncOutConfig)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            _syncOutConfig = syncOutConfig.Value;
        }

        public async Task HandleAsync(StoresRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stores requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var entidadesResponse = await _apiService.GetEntidadesAsync(token, true);
            var entidades = entidadesResponse.Result ?? new List<EntidadeResponse>();

            var payload = entidades.Select(e => new { LojaGlobalId = e.Id, NomeFantasia = e.Nome }).ToList();

            var requestUrl = $"{_syncOutConfig.ApiUrl}Settings/AtualizarInformacoesHubVindasErpExterno";
            using var httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await httpClient.PostAsync(requestUrl, content, cancellationToken);
        }
    }
}
