using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class BoletoPagamento
    {
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("identificacao")]
        public string? Identificacao { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataVencimento")]
        public string? DataVencimento { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataPagamento")]
        public string? DataPagamento { get; set; }

        [JsonProperty("codigoConta")]
        public string? CodigoConta { get; set; }
    }
}
