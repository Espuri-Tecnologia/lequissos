using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IVarejOnlineApiService _varejoOnlineApiService;
        private readonly IIntegrationService _integrationService;

        private readonly ILogger<IntegrationService> _logger;
        public AuthService(IVarejOnlineApiService varejoOnlineApiService,
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

        public async Task<Response<TokenResponse>> EnableTokenIntegrationAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("Código não informado.");

            var tokenResponse = await _varejoOnlineApiService.ExchangeCodeForTokenAsync(code);

            if (tokenResponse.IsSuccess)
            {
                var integrationDto = await _integrationService.GetIntegrationByDocument(tokenResponse.Result?.CnpjEmpresa!);
                if (integrationDto.IsSuccess)
                    await _integrationService.UpdateTokenAsync(integrationDto.Result!, tokenResponse.Result!);

                return tokenResponse!;
            }
            throw new Exception("Problema ao retornar o Token");
        }
    }
}
