namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes
{
    public class TerceiroRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public List<string> Emails { get; set; } = new();
        public string? Rg { get; set; }
        public DateTime? DataNascimento { get; set; }
        public List<EnderecoTerceiroRequest> Enderecos { get; set; } = new();
        public List<TelefoneTerceiroRequest> Telefones { get; set; } = new();
        public List<string> Classes { get; set; } = new();
    }
}
