using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.AspNetCore.Mvc;


namespace LexosHub.ERP.VarejOnline.Api.Controllers.Pedido
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidoController : Controller
    {
        private readonly IEventDispatcher _dispatcher;

        public PedidoController(IEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarPedido([FromBody] PedidoView pedido, string hubKey)
        {
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));

            var orderCreatedEvent = new OrderCreated
            {
                HubKey = hubKey,
                Pedido = pedido
            };
            await _dispatcher.DispatchAsync(orderCreatedEvent, new CancellationToken());
            return Ok();
        }

        [HttpPut("/alterar-status-entregue")]
        public async Task<IActionResult> AlterarStatusPedidoEntregue(string hubKey, long erpPedidoId)
        {
            if (string.IsNullOrWhiteSpace(hubKey)) throw new ArgumentNullException(nameof(hubKey));

            var orderCreatedEvent = new OrderDelivered
            {
                HubKey = hubKey,
                PedidoERPId = erpPedidoId
            };
            await _dispatcher.DispatchAsync(orderCreatedEvent, new CancellationToken());
            return Ok();
        }

        [HttpPut("/alterar-status-enviado")]
        public async Task<IActionResult> AlterarStatusPedidoEnviado(string hubKey, long erpPedidoId)
        {
            if (string.IsNullOrWhiteSpace(hubKey)) throw new ArgumentNullException(nameof(hubKey));

            var orderCreatedEvent = new OrderShipped
            {
                HubKey = hubKey,
                PedidoERPId = erpPedidoId
            };
            await _dispatcher.DispatchAsync(orderCreatedEvent, new CancellationToken());
            return Ok();
        }

        [HttpPost("{erpPedidoId:long}/cancelar")]
        public async Task<IActionResult> CancelarPedido(long erpPedidoId, string hubKey)
        {
            if (string.IsNullOrWhiteSpace(hubKey)) throw new ArgumentNullException(nameof(hubKey));

            var orderCancelledEvent = new OrderCancelled
            {
                HubKey = hubKey,
                PedidoERPId = erpPedidoId
            };

            await _dispatcher.DispatchAsync(orderCancelledEvent, new CancellationToken());
            return Ok();
        }
    }
}

