using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class PlanoRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("descricao")]
        public string? Descricao { get; set; }
    }
}
