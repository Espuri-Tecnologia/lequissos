using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class IntegrationCreatedEventHandler : IEventHandler<IntegrationCreated>
    {
        private readonly ILogger<IntegrationCreatedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;

        public IntegrationCreatedEventHandler(ILogger<IntegrationCreatedEventHandler> logger, IIntegrationService integrationService)
        {
            _logger = logger;
            _integrationService = integrationService;
        }

        public async Task HandleAsync(IntegrationCreated @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Integração criada: {@event.HubIntegrationId}, Cliente: {@event.Cnpj}");
            var dto = new HubIntegracaoDto
            {
                IntegracaoId = @event.HubIntegrationId,
                Chave = @event.HubKey,
                Cnpj = @event.Cnpj,
                Habilitado = true,
                Excluido = false,
                TenantId = @event.TenantId
            };

            await _integrationService.AddOrUpdateIntegrationAsync(dto);
        }
    }
}
