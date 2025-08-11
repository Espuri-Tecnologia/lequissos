using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class EnderecoEntrega
    {
        [JsonProperty("logradouro")]
        public string? Logradouro { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        [JsonProperty("bairro")]
        public string? Bairro { get; set; }

        [JsonProperty("complemento")]
        public string? Complemento { get; set; }

        [JsonProperty("cep")]
        public string? Cep { get; set; }

        [JsonProperty("cidade")]
        public string? Cidade { get; set; }

        [JsonProperty("uf")]
        public string? Uf { get; set; }

        [JsonProperty("receptorEntrega")]
        public string? ReceptorEntrega { get; set; }

        [JsonProperty("receptorEntregaDocumento")]
        public string? ReceptorEntregaDocumento { get; set; }
    }
}
