using Amazon.Runtime.Internal;
using Lexos.Hub.Sync.Models.Loja;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.SyncOut.Interfaces;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class StoresRequestedEventHandler : IEventHandler<StoresRequested>
    {
        private readonly ILogger<StoresRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly ISyncOutApiService _syncOutApiApiService;


        public StoresRequestedEventHandler(
            ILogger<StoresRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            ISyncOutApiService syncOutApiApiService)
        {
            _logger = logger;
            _apiService = apiService;
            _integrationService = integrationService;
            _syncOutApiApiService = syncOutApiApiService;
        }

        public async Task HandleAsync(StoresRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stores requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var entidadesResponse = await _apiService.GetEntidadesAsync(token);
            var entidades = entidadesResponse.Result ?? new List<EntidadeResponse>();

            var lojas = entidades.Where(x => x.EntidadeEcommerce).Select(e => new LojaDto { LojaGlobalId = e.Id, NomeFantasia = e.Nome }).ToList();

            var request = new IntegracaoErpHubDto
            {
                Chave = @event.HubKey,
                Lojas = lojas
            };
            var response = await _syncOutApiApiService.IntegrarLojas(request);

        }
    }
}
