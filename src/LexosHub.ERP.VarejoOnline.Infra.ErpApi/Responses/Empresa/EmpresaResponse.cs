using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Empresa;

namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses
{
    public class EmpresaResponse
    {
        public long Id { get; set; }
        public string DataAlteracao { get; set; } = default!;
        public string DataCriacao { get; set; } = default!;
        public string Nome { get; set; } = default!;
        public string Documento { get; set; } = default!;
        public List<EntidadeResponse> Entidades { get; set; } = new();
        public bool Ativa { get; set; }
        public bool Excluida { get; set; }
        public long IdTerceiro { get; set; }
        public string GrupoEmpresarial { get; set; } = default!;
        public bool UtilizaLinkJusta { get; set; }
        public string TokenLinkPagamentoJusta { get; set; } = default!;
        public long IdEntidadePrincipal { get; set; }
        public long IdEntidadeEcommerce { get; set; }
        public CamposCustomizadosResponse CamposCustomizados { get; set; } = new();
    }
}