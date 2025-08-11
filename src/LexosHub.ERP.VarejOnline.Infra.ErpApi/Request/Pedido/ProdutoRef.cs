using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class ProdutoRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("codigoSistema")]
        public string? CodigoSistema { get; set; }

        [JsonProperty("codigoInterno")]
        public string? CodigoInterno { get; set; }

        [JsonProperty("codigoBarras")]
        public string? CodigoBarras { get; set; }
    }
}
