namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses
{
    public class BadRequestResponse
    {
        public string code { get; set; }
        public string mensagem { get; set; }
        public string detalhes { get; set; }
        public List<BadRequestResponse> details { get; set; }
    }
}
