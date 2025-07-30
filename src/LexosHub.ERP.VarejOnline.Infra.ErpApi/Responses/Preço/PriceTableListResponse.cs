using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Preço;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices
{
    public class TabelaPrecoListResponse
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public bool? Ativo { get; set; }
        public bool? Disponivel { get; set; }
        public bool? Permanente { get; set; }
        public bool? Promocao { get; set; }
        public bool? Excluido { get; set; }
        public bool? AplicaDesconto { get; set; }
        public bool? Compra { get; set; }
        public int? Peso { get; set; }
        public string? InicioVigencia { get; set; }
        public string? FimVigencia { get; set; }
        public List<long>? IdsEntidades { get; set; }
        public List<long>? IdsClassificacoesClientes { get; set; }
        public List<ClassificacaoClienteResponse>? ClassificacoesCliente { get; set; }
        public TabelaReferenciaDescontoResponse? TabelaReferenciaDesconto { get; set; }
    }
}