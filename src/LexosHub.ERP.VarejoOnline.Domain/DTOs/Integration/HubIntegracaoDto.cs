namespace LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration
{
    public class HubIntegracaoDto
    {
        public int IntegracaoId { get; set; }
        public string? Chave { get; set; }
        public string? Cnpj { get; set; }
        public int TenantId { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }

    }
}
