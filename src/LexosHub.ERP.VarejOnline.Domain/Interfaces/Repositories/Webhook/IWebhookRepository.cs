using LexosHub.ERP.VarejOnline.Domain.DTOs.Webhook;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Webhook
{
    public interface IWebhookRepository
    {
        Task<WebhookRecordDto> AddAsync(WebhookRecordDto webhook);
    }
}
