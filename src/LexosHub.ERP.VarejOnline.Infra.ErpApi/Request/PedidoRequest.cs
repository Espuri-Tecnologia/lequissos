using System;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request
{
    /// <summary>
    /// Estrutura de pedido enviada para o Varejo Online.
    /// </summary>
    public class PedidoRequest
    {
        [JsonProperty("codigo")]
        public string Codigo { get; set; } = string.Empty;
        public string ClienteCpfCnpj { get; set; } = string.Empty;
        public string ClienteNome { get; set; } = string.Empty;
        public decimal? Total { get; set; }

        [JsonProperty("data")]
        public DateTime? Data { get; set; }
    }
}
