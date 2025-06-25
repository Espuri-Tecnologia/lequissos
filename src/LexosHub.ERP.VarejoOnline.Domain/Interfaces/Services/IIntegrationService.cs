using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Auth;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
    public interface IIntegrationService
    {
        Task<Response<IntegrationDto>> AddOrUpdateIntegrationAsync(HubIntegracaoDto item);
        Task<Response<IntegrationDto>> GetIntegrationByDocument(string cnpj);
        Task<Response<IntegrationDto>> UpdateTokenAsync(IntegrationDto integrationDto, TokenResponse tokenResponse);
    }
}
