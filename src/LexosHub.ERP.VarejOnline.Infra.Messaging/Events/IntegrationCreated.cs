namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class IntegrationCreated : BaseEvent
    {
        public int HubIntegrationId { get; set; }
        public int TenantId { get; set; }
        public string? HubKey { get; set; }
        public string? Cnpj { get; set; }
    }
}
