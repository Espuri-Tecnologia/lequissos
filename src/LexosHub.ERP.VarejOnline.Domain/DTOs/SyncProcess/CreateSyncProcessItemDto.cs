using LexosHub.ERP.VarejOnline.Domain.Enums;

namespace LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess
{
    public class CreateSyncProcessItemDto
    {
        public Guid SyncProcessId { get; set; }
        public SyncProcessTypeEnum TypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ExternalErpId { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }
        public string? JobId { get; set; }
        public SyncProcessStatusEnum InitialStatus { get; set; } = SyncProcessStatusEnum.Running;
    }
}
