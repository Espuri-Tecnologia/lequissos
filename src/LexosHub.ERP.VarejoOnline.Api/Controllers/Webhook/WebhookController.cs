using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Webhook;
using System.Text.Json;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace LexosHub.ERP.VarejoOnline.Api.Controllers.Webhook
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IVarejoOnlineApiService _varejoOnlineApiService;
        private readonly IIntegrationService _integrationService;
        private readonly IWebhookService _webhookService;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IVarejoOnlineApiService varejoOnlineApiService,
            IEventDispatcher dispatcher,
            ILogger<WebhookController> logger,
            IIntegrationService integrationService,
            IWebhookService webhookService)
        {
            _varejoOnlineApiService = varejoOnlineApiService;
            _dispatcher = dispatcher;
            _logger = logger;
            _integrationService = integrationService;
            _webhookService = webhookService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Produto([FromBody] WebhookDto webhookDto)
        {
            if (webhookDto == null)
                return BadRequest();

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(webhookDto.HubKey);

            if (integrationResponse == null)
                return BadRequest("integrationNotFound");

            var token = integrationResponse.Result?.Token ?? string.Empty;


            var request = new WebhookRequest
            {
                Event = webhookDto.Event,
                url = webhookDto.Url,
                types = new List<string> { webhookDto.Method }
            };

            var result = await _varejoOnlineApiService.RegisterWebhookAsync(token, request);

            if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Result?.Body))
            {
                var operation = JsonSerializer.Deserialize<WebhookOperationResponse>(result.Result.Body);
                if (operation != null && !string.IsNullOrWhiteSpace(operation.IdRecurso))
                {
                    await _webhookService.AddAsync(new WebhookRecordDto
                    {
                        IntegrationId = integrationResponse.Result!.Id,
                        Event = webhookDto.Event,
                        Method = webhookDto.Method,
                        Url = webhookDto.Url,
                        Uuid = operation.IdRecurso
                    });
                }
            }

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
    }
}
