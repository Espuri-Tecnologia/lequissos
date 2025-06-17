using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Auth
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public string CnpjEmpresa { get; set; } = string.Empty;
        public string IdTerceiro { get; set; } = string.Empty;
        public string NomeTerceiro { get; set; } = string.Empty;
    }
}
