namespace Lexos.Hub.Sync.Models.Produto
{
    public class VarejOnlineProdutoDto
    {
        public long id { get; set; }
        public string? descricao { get; set; }
        public string? descricaoSimplificada { get; set; }
        public string? codigoBarras { get; set; }
        public decimal? peso { get; set; }
        public decimal? comprimento { get; set; }
        public decimal? largura { get; set; }
        public decimal? altura { get; set; }
    }
}
