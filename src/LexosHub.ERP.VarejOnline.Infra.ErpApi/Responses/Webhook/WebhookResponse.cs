using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Webhook
{
    public sealed class WebhookResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Body { get; set; }
    }
}
