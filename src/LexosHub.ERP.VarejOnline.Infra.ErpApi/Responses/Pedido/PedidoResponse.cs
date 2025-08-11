using System.Text.Json.Serialization;

namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses
{
    public class PedidoResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}
