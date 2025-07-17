using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejoOnline.Domain.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IWebhookRepository _webhookRepository;

        public WebhookService(IWebhookRepository webhookRepository)
        {
            _webhookRepository = webhookRepository;
        }

        public async Task<Response<WebhookRecordDto>> AddAsync(WebhookRecordDto webhook)
        {
            var result = await _webhookRepository.AddAsync(webhook);
            return new Response<WebhookRecordDto>(result);
        }
    }
}
