namespace LexosHub.ERP.VarejoOnline.Domain.DTOs.SyncProcess
{
    public class SyncProcessDto
    {
        public Guid Id { get; set; }
        public int IntegrationId { get; set; }
        public SyncProcessTypeEnum TypeId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public bool InitialSync { get; set; }
        public SyncProcessStatusEnum StatusId { get; set; }
        public Guid? ParentId { get; set; }
        public string? AdditionalInfo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
