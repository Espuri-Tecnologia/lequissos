namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Integration
{
    public class IntegrationDto
    {
        public int Id { get; set; }
        public int HubIntegrationId { get; set; }
        public int TenantId { get; set; }
        public string? HubKey { get; set; }
        public string? Url { get; set; }
        public string? Cnpj { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? LastOrderSyncDate { get; set; }
        public bool HasValidVersion { get; set; }
    }
}
