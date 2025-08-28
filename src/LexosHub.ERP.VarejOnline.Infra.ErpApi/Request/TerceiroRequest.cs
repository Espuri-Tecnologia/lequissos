namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request
{
    public class TerceiroRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string? Contato { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Endereco { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? Pais { get; set; }
        public string? Cep { get; set; }
    }
}
