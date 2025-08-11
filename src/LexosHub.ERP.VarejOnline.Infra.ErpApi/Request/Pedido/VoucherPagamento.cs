using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class VoucherPagamento
    {
        [JsonProperty("voucher")]
        public VoucherRef? Voucher { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }
}
