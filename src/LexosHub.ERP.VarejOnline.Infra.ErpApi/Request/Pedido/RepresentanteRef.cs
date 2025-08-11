using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class RepresentanteRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("nome")]
        public string? Nome { get; set; }
    }
}
