using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class ServicoPedido
    {
        [JsonProperty("servico")]
        public ProdutoRef? Servico { get; set; }

        [JsonProperty("quantidade")]
        public decimal? Quantidade { get; set; }

        [JsonProperty("valorUnitario")]
        public decimal? ValorUnitario { get; set; }

        [JsonProperty("valorDesconto")]
        public decimal? ValorDesconto { get; set; }
    }
}
