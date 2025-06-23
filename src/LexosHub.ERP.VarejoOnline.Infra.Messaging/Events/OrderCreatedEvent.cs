namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Events
{
    public class OrderCreatedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
