using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class CartaoPagamento
    {
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("autorizacao")]
        public string? Autorizacao { get; set; }

        [JsonProperty("quantidadeParcelas")]
        public int? QuantidadeParcelas { get; set; }

        [JsonProperty("nsu")]
        public string? Nsu { get; set; }

        /// <summary>Id da negociação, se utilizada.</summary>
        [JsonProperty("negociacao")]
        public long? Negociacao { get; set; }

        /// <summary>Obrigatório se não informado negociação.</summary>
        [JsonProperty("operadoraNome")]
        public string? OperadoraNome { get; set; }

        /// <summary>Obrigatório se não informado negociação.</summary>
        [JsonProperty("bandeiraNome")]
        public string? BandeiraNome { get; set; }

        /// <summary>CREDITO ou DEBITO (quando não houver negociação).</summary>
        [JsonProperty("tipo")]
        public string? Tipo { get; set; }

        /// <summary>SEM_PARCELAMENTO, PARCELADO_VENDEDOR, PARCELADO_OPERADORA.</summary>
        [JsonProperty("parcelamento")]
        public string? Parcelamento { get; set; }
    }
}
