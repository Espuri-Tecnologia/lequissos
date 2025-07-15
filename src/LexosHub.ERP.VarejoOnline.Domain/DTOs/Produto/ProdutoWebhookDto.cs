using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;

namespace LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto
{
    public class WebhookDto
    {
        public string HubKey { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
