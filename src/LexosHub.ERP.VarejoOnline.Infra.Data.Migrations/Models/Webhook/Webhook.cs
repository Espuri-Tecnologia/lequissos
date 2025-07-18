using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Base;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Webhook
{
    public class Webhook : EntityBase
    {
        public int Id { get; set; }
        public int IntegrationId { get; set; }
        public string? Uuid { get; set; }
        public string Event { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
