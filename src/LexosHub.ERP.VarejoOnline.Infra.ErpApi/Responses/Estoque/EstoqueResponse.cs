namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses
{
    public class EstoqueResponse
    {
        public EntidadeResponse Entidade { get; set; } = new();
        public ProdutoEstoqueResponse Produto { get; set; } = new();
        public decimal Quantidade { get; set; }
        public string? DataAlteracao { get; set; }
        public string? Data { get; set; }
    }

    public class EntidadeResponse
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }

    public class ProdutoEstoqueResponse
    {
        public long Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? CodigoSistema { get; set; }
        public string? CodigoInterno { get; set; }
        public string? CodigoBarras { get; set; }
    }
}
