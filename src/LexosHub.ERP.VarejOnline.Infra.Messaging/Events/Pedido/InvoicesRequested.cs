namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class InvoicesRequested : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public long Number { get; set; }
    }
}
