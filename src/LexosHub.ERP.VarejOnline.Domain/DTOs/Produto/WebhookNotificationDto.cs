using System;

namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Produto
{
    public class WebhookNotificationDto
    {
        public string Object { get; set; } = string.Empty;
        public string WebhookEvent { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string? ContractId { get; set; }
    }
}
