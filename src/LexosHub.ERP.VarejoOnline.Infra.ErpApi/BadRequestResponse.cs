namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses
{
    public class BadRequestResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public string detailedMessage { get; set; }
        public List<BadRequestResponse> details { get; set; }
    }
}
