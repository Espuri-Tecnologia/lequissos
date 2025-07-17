using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Webhook
{
    public interface IWebhookRepository
    {
        Task<WebhookRecordDto> AddAsync(WebhookRecordDto webhook);
    }
}
