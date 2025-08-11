using System.Collections.Generic;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class CrediarioPagamento
    {
        /// <summary>Id do plano de pagamento.</summary>
        [JsonProperty("plano")]
        public long? Plano { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("valorAcrescimo")]
        public decimal? ValorAcrescimo { get; set; }

        [JsonProperty("parcelas")]
        public List<ParcelaCrediario>? Parcelas { get; set; }
    }
}
