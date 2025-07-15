using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request
{
    public sealed class WebhookRequest
    {
        public string HubKey { get; set; } = string.Empty;
        public string Event { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
