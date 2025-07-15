namespace LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings
{
    public class SyncInConfig
    {
        public string? SQSBaseUrl { get; set; }
        public string? SQSAccessKeyId { get; set; }
        public string? SQSName { get; set; }
        public int MessagesToGet { get; set; } = 10;
        public string? ApiUrl { get; set; }
    }
}
