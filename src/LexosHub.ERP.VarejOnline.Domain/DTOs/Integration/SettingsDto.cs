namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Integration
{
    public class Settings
    {
        public string? WarehouseBranchId { get; set; }
        public string? OrdersBranchId { get; set; }
        public long StatusDelivered { get; set; } 
        public long StatusShipped  { get; set; }
    }
}
