namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Events
{
    public class BaseEvent
    {
        public string EventType { get; private set; } = null!;

        public BaseEvent()
        {
            EventType = GetType().Name;
        }
    }
}
