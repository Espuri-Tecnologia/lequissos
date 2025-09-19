using FluentValidation;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LexosHub.ERP.VarejOnline.Domain.Services
{
    public class IntegrationService : IIntegrationService
    {
        private readonly IIntegrationRepository _integrationRepo;
        private readonly IValidator<HubIntegracaoDto> _validator;
        private readonly ILogger<IntegrationService> _logger;
        public IntegrationService(IIntegrationRepository integrationRepo,
                                  IValidator<HubIntegracaoDto> validator,
                                  ILogger<IntegrationService> logger)
        {
            _integrationRepo = integrationRepo;
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
                Token = item.Token,
                RefreshToken = item.RefreshToken,
                TenantId = item.TenantId,
                Cnpj = item.Cnpj,
                IsActive = item.Habilitado,
            };

            await _integrationRepo.AddAsync(integration);

            return new Response<IntegrationDto>(integration);
        }

        public async Task<Response<IntegrationDto>> UpdateTokenAsync(IntegrationDto integrationDto, TokenResponse tokenResponse)
        {
            integrationDto.Token = tokenResponse.AccessToken;
            integrationDto.RefreshToken = tokenResponse.RefreshToken;

            await _integrationRepo.UpdateAsync(integrationDto);

            return new Response<IntegrationDto>(integrationDto);
        }

        public async Task<Response<IntegrationDto>> UpdateIntegrationAsync(IntegrationDto integrationDto, HubIntegracaoDto item)
        {
            integrationDto.Url = string.Empty;
            integrationDto.IsActive = item.Habilitado;
            integrationDto.Settings = item.Settings;
            integrationDto.HubIntegrationId = item.IntegracaoId;
            integrationDto.HubKey = item.Chave;
            integrationDto.Token = item.Token;
            integrationDto.RefreshToken = item.RefreshToken;
            integrationDto.TenantId = item.TenantId;
            integrationDto.Cnpj = item.Cnpj;
            integrationDto.IsActive = item.Habilitado;

            await _integrationRepo.UpdateAsync(integrationDto);

            return new Response<IntegrationDto>(integrationDto);
        }

        public async Task<Response<IntegrationDto>> GetIntegrationByDocument(string cnpj)
        {
            return await _integrationRepo.GetByDocument(cnpj);
        }

        public async Task<Response<IntegrationDto>> GetIntegrationByKeyAsync(string hubKey)
        {
            var integration = await _integrationRepo.GetByKeyAsync(hubKey);
            return integration;
        }
    }
}
