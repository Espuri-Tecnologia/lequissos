namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoItemView
    {
        public string Sku { get; set; }
        public string Descricao { get; set; }
        public decimal Qtde { get; set; }
        public decimal Valor { get; set; }
        public decimal Desconto { get; set; }
        public decimal Acrescimo { get; set; }
        public decimal Comissao { get; set; }
        public decimal Custo { get; set; }
        public string ProdutoTipo { get; set; }
        public long? ProdutoId { get; set; }
        public long? AnuncioId { get; set; }
        public decimal ValorCustoWinthor { get; set; } = decimal.Zero;
        public bool IsKit { get; set; }
        public decimal? KitProdutoId { get; set; }

        public long? ProdutoIdGlobal { get; set; }
    }
}
