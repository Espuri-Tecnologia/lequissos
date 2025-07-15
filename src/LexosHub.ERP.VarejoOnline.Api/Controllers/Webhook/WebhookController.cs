using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Webhook
{
    [Produces("application/json")]
    [Route("api/produto")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IVarejoOnlineApiService _varejoOnlineApiService;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IVarejoOnlineApiService varejoOnlineApiService,
            IEventDispatcher dispatcher,
            ILogger<WebhookController> logger)
        {
            _varejoOnlineApiService = varejoOnlineApiService;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        [HttpPost("webhook/produto")]
        public async Task<IActionResult> Produto([FromBody] WebhookRequest produtoDto)
        {
            if (produtoDto == null)
                return BadRequest();

            await _varejoOnlineApiService.RegisterWebhookAsync(produtoDto);
            return Ok();
        }

        [HttpPost("webhook/produtos")]
        public async Task<IActionResult> Produtos([FromBody] WebhookNotificationDto notification, CancellationToken cancellationToken)
        {
            if (notification == null)
                return BadRequest();

            _logger.LogInformation("Webhook payload received: {@payload}", notification);

            long? productId = null;
            if (!string.IsNullOrWhiteSpace(notification.Object))
            {
                var lastSegment = notification.Object.TrimEnd('/')
                    .Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .LastOrDefault();
                if (long.TryParse(lastSegment, out var parsed))
                    productId = parsed;
            }

            var evt = new ProductsRequested
            {
                HubKey = notification.ContractId ?? string.Empty,
                Id = productId
            };

            await _dispatcher.DispatchAsync(evt, cancellationToken);

            _logger.LogInformation("Dispatched ProductsRequested event for product {ProductId}", productId);

            return Ok();
        }
    }
}
