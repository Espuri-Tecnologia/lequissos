using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Base;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Integration
{
    public class Integration : EntityBase
    {
        public int Id { get; set; }
        public int HubIntegrationId { get; set; }
        public int TenantId { get; set; }
        public string? HubKey { get; set; }
        public string? Cnpj { get; set; }
        public string? Url { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsActive { get; set; }
        public string? Settings { get; set; }
        public DateTime? LastSyncDate { get; set; }
        public bool HasValidVersion { get; set; }
        
    }
}
