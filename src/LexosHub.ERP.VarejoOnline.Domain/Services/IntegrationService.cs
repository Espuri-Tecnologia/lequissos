using FluentValidation;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Services;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Domain.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IIntegrationRepository _integrationRepo;
        private readonly IVarejoOnlineApiService _varejoOnlineApiService;
        private readonly IValidator<HubIntegracaoDto> _validator;
        private readonly ILogger<IntegrationService> _logger;
        public IntegrationService(IIntegrationRepository integrationRepo,
                                  IValidator<HubIntegracaoDto> validator,
                                  IVarejoOnlineApiService varejoOnlineApiService,
                                  ILogger<IntegrationService> logger)
        {
            _integrationRepo = integrationRepo; 
            _varejoOnlineApiService = varejoOnlineApiService;
            _validator = validator;
            _logger = logger;
        }
        public async Task<Response<IntegrationDto>> AddOrUpdateIntegrationAsync(HubIntegracaoDto item)
        {
            var validation = await _validator.ValidateAsync(item);

            if (!validation.IsValid)
                return new Response<IntegrationDto> { Error = new ErrorResult("Erro ao validar os dados. ", validation.Errors) };

            var integrationDto = await _integrationRepo.GetByKeyAsync(item.Chave!);

            return integrationDto is null ? await AddIntegrationAsync(item) : await UpdateIntegrationAsync(integrationDto, item);
        }
        public async Task<Response<IntegrationDto>> AddIntegrationAsync(HubIntegracaoDto item)
        {
            var integration = new IntegrationDto
            {
                HubIntegrationId = item.IntegracaoId,
                HubKey = item.Chave,
                TenantId = item.TenantId,
                IsActive = item.Habilitado,
            };

            await _integrationRepo.AddAsync(integration);

            return new Response<IntegrationDto>(integration);
        }

        public async Task<Response<IntegrationDto>> UpdateIntegrationAsync(IntegrationDto integrationDto, HubIntegracaoDto item)
        {
            integrationDto.Url = string.Empty;
            integrationDto.User = string.Empty;
            integrationDto.Password = string.Empty;
            integrationDto.IsActive = item.Habilitado;


            await _integrationRepo.UpdateAsync(integrationDto);

            return new Response<IntegrationDto>(integrationDto);
        }
    }
}
