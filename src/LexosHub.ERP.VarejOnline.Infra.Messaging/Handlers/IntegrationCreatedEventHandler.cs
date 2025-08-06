using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class IntegrationCreatedEventHandler : IEventHandler<IntegrationCreated>
    {
        private readonly ILogger<IntegrationCreatedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IEventDispatcher _dispatcher;

        public IntegrationCreatedEventHandler(ILogger<IntegrationCreatedEventHandler> logger, IIntegrationService integrationService, IEventDispatcher dispatcher)
        {
            _logger = logger;
            _integrationService = integrationService;
            _dispatcher = dispatcher;
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

            var initialSync = new InitialSync { HubKey = @event.HubKey! };
            await _dispatcher.DispatchAsync(initialSync, cancellationToken);
        }
    }
}
