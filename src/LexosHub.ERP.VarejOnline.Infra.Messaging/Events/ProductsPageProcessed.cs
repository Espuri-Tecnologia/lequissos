using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class ProductsPageProcessed : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public int Start { get; set; }
        public int PageSize { get; set; }
        public int ProcessedCount { get; set; }
        public List<ProdutoResponse>? Produtos { get; set; }
    }
}

