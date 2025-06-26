namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses
{
    public class ProdutoResponse
    {
        public long Id { get; set; }
        public bool Ativo { get; set; }
        public string? DataAlteracao { get; set; }
        public string? DataCriacao { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? DescricaoSimplificada { get; set; }
        public string? CodigoBarras { get; set; }
        public string? CodigoInterno { get; set; }
        public string? CodigoSistema { get; set; }
        public decimal? Preco { get; set; }
    }
}
