using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using System.Threading;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
    public interface IWebhookService
    {
        Task<Response<WebhookRecordDto>> AddAsync(WebhookRecordDto webhook);
        Task<Response<WebhookRecordDto>> RegisterAsync(WebhookDto webhookDto, CancellationToken cancellationToken = default);
    }
}
