using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public class OrderShippedEventHandler : IEventHandler<OrderShipped>
    {
        private readonly ILogger<OrderShippedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderShippedEventHandler(ILogger<OrderShippedEventHandler> logger, IIntegrationService integrationService, IVarejOnlineApiService apiService)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }
        public async Task HandleAsync(OrderShipped @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Alteração de Status de Pedido | PedidoERPId:  {@event.PedidoERPId}");
            if (@event != null)
            {
                var hubKey = @event.HubKey;
                var pedidoERPId = @event.PedidoERPId;

                var integration = await _integrationService.GetIntegrationByKeyAsync(hubKey);

                var token = integration.Result!.Token;

                var request = new AlterarStatusPedidoRequest
                {
                    IdPedido = @event.PedidoERPId,
                    StatusPedidoVenda = new StatusPedidoVendaRequest
                    {
                        Id = integration.Result!.Settings!.StatusShipped,
                    }
                };

                var result = _apiService.AlterarStatusPedidoAsync(token!, request);

                if (!result.IsCompleted)
                {
                    //tratar erro do processo
                }


            }
        }
    }
}
