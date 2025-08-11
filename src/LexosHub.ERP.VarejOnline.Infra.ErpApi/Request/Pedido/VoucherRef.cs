using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class VoucherRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
    }
}
