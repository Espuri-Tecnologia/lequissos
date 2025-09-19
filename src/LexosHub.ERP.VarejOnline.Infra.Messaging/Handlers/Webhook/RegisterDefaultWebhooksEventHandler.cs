using LexosHub.ERP.VarejOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class RegisterDefaultWebhooksEventHandler : IEventHandler<RegisterDefaultWebhooks>
    {
        private readonly ILogger<RegisterDefaultWebhooksEventHandler> _logger;
        private readonly IWebhookService _webhookService;

        public RegisterDefaultWebhooksEventHandler(
            ILogger<RegisterDefaultWebhooksEventHandler> logger,
            IWebhookService webhookService)
        {
            _logger = logger;
            _webhookService = webhookService;
        }

        public async Task HandleAsync(RegisterDefaultWebhooks @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering default webhooks for hub {HubKey}", @event.HubKey);

            var events = new[] { "PRODUTOS", "TABELAPRECOPRODUTO", "NOTAFISCAL" };
            var methods = new List<string> { "POST", "PUT" };

            foreach (var evt in events)
            {
                foreach (var method in methods)
                {
                    var webhook = new WebhookDto
                    {
                        HubKey = @event.HubKey,
                        Event = evt,
                        Types = methods,
                        Url = $"https://api-varejoonline.lexoshub.com/{@event.HubKey}/{evt}"
                    };

                    await _webhookService.RegisterAsync(webhook, cancellationToken);
                }
            }
        }
    }
}
