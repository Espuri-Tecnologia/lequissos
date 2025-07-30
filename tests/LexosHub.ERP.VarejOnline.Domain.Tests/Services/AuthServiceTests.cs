using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<ILogger<IntegrationService>> _logger = new();

        private AuthService CreateService() => new AuthService(_apiService.Object, _logger.Object, _integrationService.Object);

        [Fact]
        public async Task EnableTokenIntegrationAsync_WithEmptyCode_ShouldThrow()
        {
            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.EnableTokenIntegrationAsync(string.Empty));
        }

        [Fact]
        public async Task EnableTokenIntegrationAsync_WhenSuccess_ShouldUpdateToken()
        {
            var tokenResponse = new Response<TokenResponse>(new TokenResponse { CnpjEmpresa = "1" });
            var integrationResponse = new Response<IntegrationDto>(new IntegrationDto());
            _apiService.Setup(a => a.ExchangeCodeForTokenAsync("code")).ReturnsAsync(tokenResponse);
            _integrationService.Setup(i => i.GetIntegrationByDocument("1")).ReturnsAsync(integrationResponse);
            _integrationService.Setup(i => i.UpdateTokenAsync(It.IsAny<IntegrationDto>(), tokenResponse.Result)).ReturnsAsync(integrationResponse);
            var service = CreateService();

            var response = await service.EnableTokenIntegrationAsync("code");

            Assert.True(response.IsSuccess);
            _integrationService.Verify(i => i.UpdateTokenAsync(It.IsAny<IntegrationDto>(), tokenResponse.Result), Times.Once);
        }
    }
}
