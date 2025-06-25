namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request
{
    public class EmpresaRequest
    {
        public int? Inicio { get; set; }
        public int? Quantidade { get; set; }
        public string? AlteradoApos { get; set; }
        public string? Status { get; set; }
        public string? CampoCustomizadoNome { get; set; }
        public string? CampoCustomizadoValor { get; set; }
        public string? Cnpj { get; set; }
    }
}