using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        [HttpPut("{pedidoNumero}/status/{novoStatus}")]
        public async Task<IActionResult> AlterarStatusPedido(string hubKey, long pedidoNumero, string novoStatus)
        {
            var result = await _pedidoService.AlterarStatusPedido(hubKey, pedidoNumero, novoStatus);
            if (result.IsSuccess)
                return Ok(result);

            if (result.StatusCode == HttpStatusCode.Conflict)
                return Conflict(result);

            return BadRequest(result);
        }
    }
}

