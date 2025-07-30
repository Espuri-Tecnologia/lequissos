using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class PriceTablesRequested : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public int Start { get; set; }
        public int PageSize { get; set; }
        public int ProcessedCount { get; set; }
        public List<TabelaPrecoListResponse>? PriceTables { get; set; }
    }
}
