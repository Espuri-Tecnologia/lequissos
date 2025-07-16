namespace Lexos.Hub.Sync.Models.Produto
{
    public class ImportJobImagemView
    {
        public string Url { get; set; }
        public bool IsThumbnail { get; set; }
        public short Position { get; set; }
        public long? AtributoOpcaoId { get; set; }
        public long? ProdutoIdVariacao { get; set; }
        public string SkuVariacao { get; set; }
    }
}