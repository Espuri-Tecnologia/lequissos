using LexosHub.ERP.VarejOnline.Domain.Enums;

namespace LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess
{
    public class SyncProcessItemDto
    {
        public int Id { get; set; }
        public Guid SyncProcessId { get; set; }
        public SyncProcessTypeEnum TypeId { get; set; }
        public SyncProcessStatusEnum StatusId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ExternalErpId { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }
        public string? JobId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public SyncProcessItemDto()
        {
        }

        public SyncProcessItemDto(Guid syncProcessId, SyncProcessTypeEnum typeId, string description, string externalErpId, SyncProcessStatusEnum status = SyncProcessStatusEnum.Running)
        {
            SyncProcessId = syncProcessId;
            TypeId = typeId;
            Description = description;
            ExternalErpId = externalErpId;
            StatusId = status;
        }
    }
}
