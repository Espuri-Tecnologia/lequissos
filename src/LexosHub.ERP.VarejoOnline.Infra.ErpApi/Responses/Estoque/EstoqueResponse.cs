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
}
