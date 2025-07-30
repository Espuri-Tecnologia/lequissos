namespace LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings
{
    public class VarejOnlineApiSettings
    {
        public string OAuthUrl { get; set; }
        public string BaseUrl { get; set; }
        public string? OAuthGetTokenUrl { get; set; }
        public string? OAuthRedirectUrl { get; set; }
        public string? WebhookEndpoint { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}