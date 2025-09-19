namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class StocksRequested : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public int? Inicio { get; set; }
        public int? Quantidade { get; set; }
        public string? Produtos { get; set; }
        public string? Entidades { get; set; }
        public string? AlteradoApos { get; set; }
        public string? Data { get; set; }
        public bool? SomenteEcommerce { get; set; }
        public bool? SomenteMarketplace { get; set; }
    }
}