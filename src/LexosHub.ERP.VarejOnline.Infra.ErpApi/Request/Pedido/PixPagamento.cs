using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class PixPagamento
    {
        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataPagamento")]
        public string? DataPagamento { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("nsu")]
        public string? Nsu { get; set; }

        [JsonProperty("autorizacao")]
        public string? Autorizacao { get; set; }
    }
}
