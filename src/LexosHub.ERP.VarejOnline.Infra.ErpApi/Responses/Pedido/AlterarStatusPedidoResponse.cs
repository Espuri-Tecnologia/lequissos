using System.Text.Json.Serialization;

namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses
{
    public class AlterarStatusPedidoResponse
    {
        [JsonPropertyName("idPedido")]
        public long IdPedido { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("statusPedidoVenda")]
        public StatusPedidoVendaResponse? StatusPedidoVenda { get; set; }
    }

    public class StatusPedidoVendaResponse
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("nome")]
        public string? Nome { get; set; }
    }
}
