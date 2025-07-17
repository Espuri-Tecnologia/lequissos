using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
    public interface IWebhookService
    {
        Task<Response<WebhookRecordDto>> AddAsync(WebhookRecordDto webhook);
    }
}
