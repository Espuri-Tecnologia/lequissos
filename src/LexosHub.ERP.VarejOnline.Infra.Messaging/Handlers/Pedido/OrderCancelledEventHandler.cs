using System.Threading;
using System.Threading.Tasks;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public class OrderCancelledEventHandler : IEventHandler<OrderCancelled>
    {
        private readonly ILogger<OrderCancelledEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderCancelledEventHandler(
            ILogger<OrderCancelledEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task HandleAsync(OrderCancelled @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Cancelamento de Pedido | PedidoERPId:  {@event.PedidoERPId}");

            if (@event != null)
            {
                var integration = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);

                var token = integration.Result!.Token;

                var response = await _apiService.CancelarPedidoAsync(token!, @event.PedidoERPId);

                if (!response.IsSuccess)
                {
                    //tratar erro do processo
                }
            }
        }
    }
}
