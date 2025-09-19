namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Integration
{
    public class HubIntegracaoDto
    {
        public int IntegracaoId { get; set; }
        public string? Chave { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Cnpj { get; set; }
        public int TenantId { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public Settings? Settings { get; set; }

    }
}
