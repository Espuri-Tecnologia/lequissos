using LexosHub.ERP.VarejOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using System.Threading;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface IWebhookService
    {
        Task<Response<WebhookRecordDto>> AddAsync(WebhookRecordDto webhook);
        /// <summary>
        /// Register a webhook for the given hub key and event using the provided types.
        /// </summary>
        Task<Response<WebhookRecordDto>> RegisterAsync(WebhookDto webhookDto, CancellationToken cancellationToken = default);
    }
}
