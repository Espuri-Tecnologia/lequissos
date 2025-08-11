using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejOnline.Api.Controllers.Pedido
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost]
        public async Task<IActionResult> EnviarPedido([FromBody] PedidoView pedido, string hubKey)
        {
            var result = await _pedidoService.EnviarPedido(hubKey, pedido);
            return Ok(result);
        }
    }
}

