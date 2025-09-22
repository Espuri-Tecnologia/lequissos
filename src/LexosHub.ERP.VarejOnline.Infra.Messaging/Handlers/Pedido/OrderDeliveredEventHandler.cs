using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public class OrderDeliveredEventHandler : IEventHandler<OrderDelivered>
    {
        private readonly ILogger<OrderDeliveredEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderDeliveredEventHandler(ILogger<OrderDeliveredEventHandler> logger, IIntegrationService integrationService, IVarejOnlineApiService apiService)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }
        public async Task HandleAsync(OrderDelivered @event, CancellationToken cancellationToken)
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
                        Id = integration.Result!.Settings!.StatusDelivered,
                    }
                };

                await _apiService.AlterarStatusPedidoAsync(token!, request);
            }
        }
    }
}
