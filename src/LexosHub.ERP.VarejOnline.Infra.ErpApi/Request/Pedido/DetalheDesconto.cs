using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class DetalheDesconto
    {
        [JsonProperty("observacao")]
        public string? Observacao { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        /// <summary>Origem do desconto (texto conforme wiki).</summary>
        [JsonProperty("origemDesconto")]
        public string? OrigemDesconto { get; set; }
    }
}
