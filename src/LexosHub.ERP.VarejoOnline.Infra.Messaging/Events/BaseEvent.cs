namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Events
{
    public abstract class BaseEvent
    {
        public string EventType { get; set; } = null!;
    }
}
