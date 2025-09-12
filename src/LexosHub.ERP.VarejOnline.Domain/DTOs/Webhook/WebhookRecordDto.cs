using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Webhook
{
    public class WebhookRecordDto
    {
        public int Id { get; set; }
        public int IntegrationId { get; set; }
        public string? Uuid { get; set; }
        public string Event { get; set; } = string.Empty;
        public List<string> Types { get; set; } = new();
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
