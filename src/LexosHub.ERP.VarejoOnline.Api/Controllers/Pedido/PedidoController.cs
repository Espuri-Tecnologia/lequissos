using LexosHub.ERP.VarejoOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Pedido
{
    [Produces("application/json")]
    [Route("api/pedido")]
    [ApiController]
    public class PedidoController : Controller
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] PedidoWebhookDto pedidoDto)
        {
            if (pedidoDto == null)
                return BadRequest();

            await _pedidoService.ProcessWebhookAsync(pedidoDto);
            return Ok();
        }
    }
}
