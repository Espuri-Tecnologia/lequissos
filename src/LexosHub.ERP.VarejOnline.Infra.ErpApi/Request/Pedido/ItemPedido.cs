using System.Collections.Generic;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class ItemPedido
    {
        [JsonProperty("produto")]
        public ProdutoRef? Produto { get; set; }

        [JsonProperty("quantidade")]
        public decimal? Quantidade { get; set; }

        [JsonProperty("valorDesconto")]
        public decimal? ValorDesconto { get; set; }

        [JsonProperty("valesPresentes")]
        public List<ValePresente>? ValesPresentes { get; set; }

        [JsonProperty("descontoDetalhes")]
        public List<DetalheDesconto>? DescontoDetalhes { get; set; }

        /// <summary>Valor unitário antes do desconto.</summary>
        [JsonProperty("valorUnitario")]
        public decimal? ValorUnitario { get; set; }

        /// <summary>Id da operação do item.</summary>
        [JsonProperty("operacao")]
        public long? Operacao { get; set; }

        /// <summary>CFOP do item (quando aplicável).</summary>
        [JsonProperty("cfop")]
        public string? Cfop { get; set; }
    }
}
