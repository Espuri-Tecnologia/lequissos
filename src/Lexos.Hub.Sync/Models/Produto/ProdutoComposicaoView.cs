namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoComposicaoView
    {
        public long ProdutoId { get; set; }
        public long ProdutoIdGlobal { get; set; }
        public long ProdutoCompostoId { get; set; }
        public long ProdutoCompostoIdGlobal { get; set; }
        public double Quantidade { get; set; }
        public string Tipo { get; set; }
        public string Sku { get; set; }
    }
}