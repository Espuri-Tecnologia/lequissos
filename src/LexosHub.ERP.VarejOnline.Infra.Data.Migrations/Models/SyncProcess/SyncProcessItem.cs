using LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Models.Base;

namespace LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Models.SyncProcess
{
    public class SyncProcessItem : EntityBase
    {
        public int Id { get; set; }
        public Guid SyncProcessId { get; set; }
        public int TypeId { get; set; }
        public int StatusId { get; set; }
        public string Description { get; set; } = null!;
        public string ExternalErpId { get; set; } = null!;
        public string? AdditionalInfo { get; set; }
        public string? JobId { get; set; }

        public SyncProcess SyncProcess { get; set; } = null!;
    }
}
