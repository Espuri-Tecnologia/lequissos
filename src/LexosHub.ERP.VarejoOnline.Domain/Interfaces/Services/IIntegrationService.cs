using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
    public interface IIntegrationService
    {
        Task<Response<IntegrationDto>> AddOrUpdateIntegrationAsync(HubIntegracaoDto item);
    }
}
