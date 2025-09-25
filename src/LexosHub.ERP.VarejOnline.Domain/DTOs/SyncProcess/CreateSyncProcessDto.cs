using LexosHub.ERP.VarejOnline.Domain.Enums;

namespace LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess
{
    public class CreateSyncProcessDto
    {
        public int IntegrationId { get; set; }
        public SyncProcessTypeEnum TypeId { get; set; }
        public DateTime ReferenceDate { get; set; } = DateTime.UtcNow;
        public int PageSize { get; set; }
        public int Page { get; set; }
        public bool InitialSync { get; set; }
        public Guid? ParentId { get; set; }
        public string? AdditionalInfo { get; set; }
        public SyncProcessStatusEnum InitialStatus { get; set; } = SyncProcessStatusEnum.Running;
    }
}
