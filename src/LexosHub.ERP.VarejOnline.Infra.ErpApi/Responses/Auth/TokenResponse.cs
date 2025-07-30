using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth
{
    public class TokenResponse
    {
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonProperty("cnpj_empresa")] 
    public string CnpjEmpresa { get; set; } = string.Empty; 
    [JsonProperty("id_terceiro")]
    public string IdTerceiro { get; set; } = string.Empty; 
    [JsonProperty("nome_terceiro")]
    public string NomeTerceiro { get; set; } = string.Empty;
    }
}
