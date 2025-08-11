using System;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request
{
    public class PedidoRequest
    {
        [JsonProperty("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonProperty("data")]
        public DateTime? Data { get; set; }
    }
}
