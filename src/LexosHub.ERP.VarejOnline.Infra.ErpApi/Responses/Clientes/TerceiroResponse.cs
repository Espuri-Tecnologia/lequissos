using Newtonsoft.Json;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses.Clientes
{
    public class TerceiroResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonProperty("documento")]
        public string Documento { get; set; } = string.Empty;
        [JsonProperty("tipo")]
        public string Tipo { get; set; } = string.Empty;
    }

    public class TerceiroContatoResponse
    {
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
    }

    public class TerceiroEnderecoResponse
    {
        public string? Tipo { get; set; }
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Pais { get; set; }
        public string? Cep { get; set; }
        public string? Complemento { get; set; }
    }
}
