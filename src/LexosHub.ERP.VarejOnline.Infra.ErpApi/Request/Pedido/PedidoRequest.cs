using System.Text.Json.Serialization;

namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request
{
    public class PedidoRequest
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;
    }
}
