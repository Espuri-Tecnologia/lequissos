namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class RegisterDefaultWebhooks : BaseEvent
    {
        public string HubKey { get; set; } = null!;
    }
}
