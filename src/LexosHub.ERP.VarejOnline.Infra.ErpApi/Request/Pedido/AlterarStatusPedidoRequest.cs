using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class AlterarStatusPedidoRequest
    {
        [JsonProperty("idPedido")]
        public long IdPedido { get; set; }

        [JsonProperty("statusPedidoVenda")]
        public StatusPedidoVendaRequest? StatusPedidoVenda { get; set; }
    }

    public class StatusPedidoVendaRequest
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
    }
}
