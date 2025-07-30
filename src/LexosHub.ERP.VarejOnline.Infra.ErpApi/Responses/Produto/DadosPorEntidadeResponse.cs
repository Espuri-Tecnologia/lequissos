namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses
{
    public class DadosPorEntidadeResponse
    {
        public long Entidade { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal EstoqueMaximo { get; set; }
        public string? CodBeneficioFiscal { get; set; }
    }
}
