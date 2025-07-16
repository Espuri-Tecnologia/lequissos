namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoImpostoView
    {
        public long ProdutoIdGlobal { get; set; }
        public string NCM { get; set; }
        public string Cest { get; set; }
        public short Origem { get; set; }
        public decimal AliqImportacao { get; set; }
        public decimal AliqNacional { get; set; }
        public decimal AliqEstadual { get; set; }
        public decimal AliqMunicipal { get; set; }
        public string CodigoAnvisa { get; set; }
        public decimal ValorMaximoConsumidor { get; set; }
    }
}
