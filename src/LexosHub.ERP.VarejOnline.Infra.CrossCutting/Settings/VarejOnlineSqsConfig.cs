namespace LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings
{
    public class VarejOnlineSqsConfig
    {
        public string? SQSBaseUrl { get; set; } = string.Empty;
        public string? SQSAccessKeyId { get; set; } = string.Empty;
        public Dictionary<string, string> SQSQueues { get; set; } = new();
    }
}