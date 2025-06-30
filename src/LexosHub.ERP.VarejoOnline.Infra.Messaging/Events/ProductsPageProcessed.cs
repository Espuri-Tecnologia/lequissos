namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Events
{
    public class ProductsPageProcessed : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public int Start { get; set; }
        public int PageSize { get; set; }
        public int ProcessedCount { get; set; }
    }
}

