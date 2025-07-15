using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Webhook
{
    [Produces("application/json")]
    [Route("api/produto")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IVarejoOnlineApiService _varejoOnlineApiService;

        public WebhookController(IVarejoOnlineApiService varejoOnlineApiService)
        {
            _varejoOnlineApiService = varejoOnlineApiService;
        }

        [HttpPost("webhook/produto")]
        public async Task<IActionResult> Produto([FromBody] WebhookRequest produtoDto)
        {
            if (produtoDto == null)
                return BadRequest();

            await _varejoOnlineApiService.RegisterWebhookAsync(produtoDto);
            return Ok();
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookRequest produtoDto)
        {
            if (produtoDto == null)
                return BadRequest();

            await _varejoOnlineApiService.RegisterWebhookAsync(produtoDto);
            return Ok();
        }
    }
}
