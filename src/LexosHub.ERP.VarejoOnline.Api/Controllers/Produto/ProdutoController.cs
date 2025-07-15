using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Produto
{
    [Produces("application/json")]
    [Route("api/produto")]
    [ApiController]
    public class ProdutoController : Controller
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] ProdutoWebhookDto produtoDto)
        {
            if (produtoDto == null)
                return BadRequest();

            await _produtoService.ProcessWebhookAsync(produtoDto);
            return Ok();
        }
    }
}
