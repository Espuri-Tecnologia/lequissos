namespace LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Empresa
{
    public class CamposCustomizadosResponse
    {
        public long Id { get; set; }
        public List<ValorPrimitivoResponse> ValoresPrimitivo { get; set; } = new();
    }

    public class ValorPrimitivoResponse
    {
        public long Id { get; set; }
        public object Value { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}
