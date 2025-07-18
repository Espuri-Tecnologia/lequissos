using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Base;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.SyncProcess
{
    public class SyncProcess : EntityBase
    {
        public Guid Id { get; set; }
        public int IntegrationId { get; set; }
        public int TypeId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public bool InitialSync { get; set; }
        public int StatusId { get; set; }
        public Guid? ParentId { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
