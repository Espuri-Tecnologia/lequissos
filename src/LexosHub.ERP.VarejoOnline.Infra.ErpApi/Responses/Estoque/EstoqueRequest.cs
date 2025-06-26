namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request
{
    public class EstoqueRequest
    {
        public string? Produtos { get; set; }
        public string? Entidades { get; set; }
        public int? Inicio { get; set; }
        public int? Quantidade { get; set; }
        public string? AlteradoApos { get; set; }
        public string? Data { get; set; }
        public bool? SomenteEcommerce { get; set; }
        public bool? SomenteMarketplace { get; set; }
    }
}
