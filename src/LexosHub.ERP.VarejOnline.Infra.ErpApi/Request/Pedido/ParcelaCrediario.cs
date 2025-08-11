using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class ParcelaCrediario
    {
        [JsonProperty("numero")]
        public long? Numero { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("vencimento")]
        public string? Vencimento { get; set; }
    }
}
