using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IVarejoOnlineApiService _varejoOnlineApiService;
        private readonly IIntegrationService _integrationService;

        private readonly ILogger<IntegrationService> _logger;
        public AuthService(IVarejoOnlineApiService varejoOnlineApiService,
                                  ILogger<IntegrationService> logger,
                                  IIntegrationService integrationService)
        {
            _varejoOnlineApiService = varejoOnlineApiService;
            _logger = logger;
            _integrationService = integrationService;
        }

        public async Task<string> GetAuthUrl()
        {
            return await _varejoOnlineApiService.GetAuthUrl();
        }

        public async Task<Response<IntegrationDto>> EnableTokenIntegrationAsync(string code)
        {

            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("Código não informado.");

            var tokenResponse = await _varejoOnlineApiService.ExchangeCodeForTokenAsync(code);

            if (tokenResponse.IsSuccess)
            {
                var integrationDto = await _integrationService.GetIntegrationByDocument(tokenResponse.Result?.CnpjEmpresa);
                if (!integrationDto.IsSuccess)
                    throw new ArgumentNullException("Empresa não cadastrada na integração");

                return await _integrationService.UpdateTokenAsync(integrationDto.Result, tokenResponse.Result);
            }
            throw new Exception("Problema ao retornar o Token");
        }
    }
}
