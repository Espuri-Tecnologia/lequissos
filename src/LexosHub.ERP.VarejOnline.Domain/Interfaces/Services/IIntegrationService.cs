using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface IIntegrationService
    {
        Task<Response<IntegrationDto>> AddOrUpdateIntegrationAsync(HubIntegracaoDto item);
        Task<Response<IntegrationDto>> GetIntegrationByDocument(string cnpj);
        Task<Response<IntegrationDto>> GetIntegrationByKeyAsync(string hubKey);
        Task<Response<IntegrationDto>> UpdateTokenAsync(IntegrationDto integrationDto, TokenResponse tokenResponse);
    }
}
