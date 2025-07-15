using Newtonsoft.Json;

namespace LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request
{
    public sealed class WebhookRequest
    {
        [JsonProperty("types")]
        public List<string> types { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; } = string.Empty;

        [JsonProperty("url")]
        public string url { get; set; } = string.Empty;

    }
}
