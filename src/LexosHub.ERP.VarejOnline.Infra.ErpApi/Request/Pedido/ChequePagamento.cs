using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class ChequePagamento
    {
        [JsonProperty("titular")]
        public long? Titular { get; set; }

        [JsonProperty("banco")]
        public long? Banco { get; set; }

        [JsonProperty("agencia")]
        public string? Agencia { get; set; }

        [JsonProperty("conta")]
        public string? Conta { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataEmissao")]
        public string? DataEmissao { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataVencimento")]
        public string? DataVencimento { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }
}
