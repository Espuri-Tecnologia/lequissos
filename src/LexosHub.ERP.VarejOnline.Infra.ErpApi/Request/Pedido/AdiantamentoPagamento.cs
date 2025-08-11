using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class AdiantamentoPagamento
    {
        /// <summary>Id do adiantamento recebido.</summary>
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }
}
