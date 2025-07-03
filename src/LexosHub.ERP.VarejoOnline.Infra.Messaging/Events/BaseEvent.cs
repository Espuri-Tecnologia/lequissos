namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Events
{
    public class BaseEvent
    {
        public string EventType { get; private set; } = null!;

        protected BaseEvent()
        {
            EventType = GetType().Name;
        }
    }
}
