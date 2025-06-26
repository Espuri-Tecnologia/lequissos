namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Events
{
    public class ProductsRequested : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public long? Id { get; set; }
        public int? Inicio { get; set; }
        public int? Quantidade { get; set; }
        public string? AlteradoApos { get; set; }
        public string? Categoria { get; set; }
        public long? ProdutoBase { get; set; }
        public string? Descricao { get; set; }
        public string? CodigoBarras { get; set; }
        public string? CodigoInterno { get; set; }
        public string? CodigoSistema { get; set; }
        public bool? SomenteAtivos { get; set; }
        public bool? SomenteComFotos { get; set; }
        public bool? SomenteEcommerce { get; set; }
        public bool? SomenteMarketplace { get; set; }
        public bool? AmostraGratis { get; set; }
        public string? AlteracaoDesde { get; set; }
        public string? AlteracaoAte { get; set; }
        public string? CriacaoDesde { get; set; }
        public string? CriacaoAte { get; set; }
        public string? IdsProdutos { get; set; }
        public string? IdsTabelasPrecos { get; set; }
    }
}
