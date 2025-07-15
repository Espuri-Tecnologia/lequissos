using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Webhook
{
    [Produces("application/json")]
    [Route("api/produto")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IProdutoService _produtoService;

        public WebhookController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost("webhook/produto")]
        public async Task<IActionResult> Produto([FromBody] ProdutoWebhookDto produtoDto)
        {
            if (produtoDto == null)
                return BadRequest();

            await _produtoService.ProcessWebhookAsync(produtoDto);
            return Ok();
        }
    }
}
