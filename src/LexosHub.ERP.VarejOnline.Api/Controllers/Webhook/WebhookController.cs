using LexosHub.ERP.VarejOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejOnline.Api.Controllers.Webhook
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IWebhookService _webhookService;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IEventDispatcher dispatcher,
            ILogger<WebhookController> logger,
            IWebhookService webhookService)
        {
            _dispatcher = dispatcher;
            _logger = logger;
            _webhookService = webhookService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Produto([FromBody] WebhookDto webhookDto)
        {
            if (webhookDto == null)
                return BadRequest();

            await _webhookService.RegisterAsync(webhookDto);

            return Ok();
        }

        [HttpPost("{hubkey}/produto")]
        public async Task<IActionResult> Produtos([FromBody] WebhookNotificationDto notification, [FromRoute] string hubkey, CancellationToken cancellationToken)
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
                HubKey = hubkey ?? string.Empty,
                Id = productId
            };

            await _dispatcher.DispatchAsync(evt, cancellationToken);

            _logger.LogInformation("Dispatched ProductsRequested event for product {ProductId}", productId);

            return Ok();
        }
        [HttpPost("{hubkey}/precoproduto")]
        public async Task<IActionResult> PrecoProduto([FromBody] WebhookNotificationDto notification, [FromRoute] string hubkey, CancellationToken cancellationToken)
        {
            if (notification == null)
                return BadRequest();

            //_logger.LogInformation("Webhook payload received: {@payload}", notification);

            //long? productId = null;
            //if (!string.IsNullOrWhiteSpace(notification.Object))
            //{
            //    var lastSegment = notification.Object.TrimEnd('/')
            //        .Split('/', StringSplitOptions.RemoveEmptyEntries)
            //        .LastOrDefault();
            //    if (long.TryParse(lastSegment, out var parsed))
            //        productId = parsed;
            //}

            //var evt = new ProductsRequested
            //{
            //    HubKey = hubkey ?? string.Empty,
            //    Id = productId
            //};

            //await _dispatcher.DispatchAsync(evt, cancellationToken);


            return Ok();
        }
    }
}
