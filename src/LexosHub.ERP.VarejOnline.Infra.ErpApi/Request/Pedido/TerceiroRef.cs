using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class TerceiroRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("documento")]
        public string? Documento { get; set; }
    }
}
