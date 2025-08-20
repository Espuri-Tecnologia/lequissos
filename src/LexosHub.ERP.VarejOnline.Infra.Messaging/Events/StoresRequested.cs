namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class StoresRequested : BaseEvent
    {
        public string HubKey { get; set; } = null!;
    }
}
