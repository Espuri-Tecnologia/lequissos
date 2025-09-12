using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Produto
{
    public class WebhookDto
    {
        public string HubKey { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public List<string> Types { get; set; } = new();
        public string Url { get; set; } = string.Empty;
    }
}
