namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses
{
    public class ComponenteResponse
    {
        public ComponenteProdutoResponse Produto { get; set; } = new();
        public decimal Quantidade { get; set; }
        public string? Unidade { get; set; }
    }
}
