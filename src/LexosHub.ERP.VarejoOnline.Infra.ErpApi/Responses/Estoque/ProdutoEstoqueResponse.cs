namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses
{
    public class ProdutoEstoqueResponse
    {
        public long Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? CodigoSistema { get; set; }
        public string? CodigoInterno { get; set; }
        public string? CodigoBarras { get; set; }
    }
}
