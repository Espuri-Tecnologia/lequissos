namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Webhook
{
    public class WebhookRecordDto
    {
        public int Id { get; set; }
        public int IntegrationId { get; set; }
        public string? Uuid { get; set; }
        public string Event { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
