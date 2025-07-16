using System.Collections.Generic;

namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoImagemView
    {
        public List<ImportJobImagemView> Imagens { get; set; } = new List<ImportJobImagemView>();
        public string TipoProdutoId { get; set; }
        public string Sku { get; set; }
        public long? ProdutoIdGlobal { get; set; }
        public long? ProdutoId { get; set; }
        public bool Sobrescrever { get; set; } = false;
    }
}
