using System.Collections.Generic;

namespace LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Empresa
{
    public class CamposCustomizadosResponse
    {
        public long Id { get; set; }
        public List<ValorPrimitivoResponse> ValoresPrimitivo { get; set; } = new();
    }
}
