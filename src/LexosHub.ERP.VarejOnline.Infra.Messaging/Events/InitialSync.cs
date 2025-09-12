namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class InitialSync : BaseEvent
    {
        public string HubKey { get; set; } = null!;
    }
}
