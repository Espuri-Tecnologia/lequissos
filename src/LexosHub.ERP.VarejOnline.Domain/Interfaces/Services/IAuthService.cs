using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Response<TokenResponse>> EnableTokenIntegrationAsync(string code);
        Task<string> GetAuthUrl();
    }
}
