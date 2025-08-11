using System.Collections.Generic;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class Pagamento
    {
        [JsonProperty("valorDinheiro")]
        public decimal? ValorDinheiro { get; set; }

        [JsonProperty("cartoes")]
        public List<CartaoPagamento>? Cartoes { get; set; }

        [JsonProperty("cheques")]
        public List<ChequePagamento>? Cheques { get; set; }

        [JsonProperty("adiantamentos")]
        public List<AdiantamentoPagamento>? Adiantamentos { get; set; }

        [JsonProperty("crediario")]
        public CrediarioPagamento? Crediario { get; set; }

        [JsonProperty("boletos")]
        public List<BoletoPagamento>? Boletos { get; set; }

        [JsonProperty("vouchers")]
        public List<VoucherPagamento>? Vouchers { get; set; }

        [JsonProperty("pixes")]
        public List<PixPagamento>? Pixes { get; set; }
    }
}
