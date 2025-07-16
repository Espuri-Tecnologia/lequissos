namespace Lexos.Hub.Sync.Models.Produto
{
    public class AnuncioView
    {
        public long AnuncioId { get; set; }
        public long? AnuncioPaiId { get; set; }
        public int IntegracaoId { get; set; }
        public bool IsNew { get; set; }
        public string TipoId { get; set; }
        public string Status { get; set; }
        public short? PlataformaId { get; set; }
        public string CodigoPlataforma { get; set; }
        public bool Sincronizar { get; set; }
        public long? ProdutoId { get; set; }
        public decimal? Preco { get; set; }
    }
}
