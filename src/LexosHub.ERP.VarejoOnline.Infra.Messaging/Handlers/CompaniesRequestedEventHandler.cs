using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class CompaniesRequestedEventHandler : IEventHandler<CompaniesRequested>
    {
        private readonly ILogger<CompaniesRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejoOnlineApiService _apiService;

        public CompaniesRequestedEventHandler(
            ILogger<CompaniesRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejoOnlineApiService apiService)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task HandleAsync(CompaniesRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Companies requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var request = new EmpresaRequest
            {
                Inicio = @event.Inicio,
                Quantidade = @event.Quantidade,
                AlteradoApos = @event.AlteradoApos,
                Status = @event.Status,
                CampoCustomizadoNome = @event.CampoCustomizadoNome,
                CampoCustomizadoValor = @event.CampoCustomizadoValor,
                Cnpj = @event.Cnpj
            };

            await _apiService.GetEmpresasAsync(token, request);
        }
    }
}
