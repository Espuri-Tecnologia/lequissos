using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Services
{
    public class IntegrationServiceTests
    {
        private readonly Mock<IIntegrationRepository> _repo = new();
        private readonly Mock<IValidator<HubIntegracaoDto>> _validator = new();
        private readonly Mock<ILogger<IntegrationService>> _logger = new();

        private IntegrationService CreateService() => new IntegrationService(_repo.Object, _validator.Object, _logger.Object);

        [Fact]
        public async Task AddIntegrationAsync_ShouldReturnResponseWithIntegration()
        {
            var dto = new HubIntegracaoDto { IntegracaoId = 1, Chave = "key", TenantId = 2, Cnpj = "1", Habilitado = true };
            var service = CreateService();

            _repo.Setup(r => r.AddAsync(It.IsAny<IntegrationDto>()))
                .Callback<IntegrationDto>(i => i.Id = 10)
                .ReturnsAsync((IntegrationDto i) => i);

            var response = await service.AddIntegrationAsync(dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(dto.Chave, response.Result?.HubKey);
            Assert.Equal(10, response.Result?.Id);
            _repo.Verify(r => r.AddAsync(It.IsAny<IntegrationDto>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTokenAsync_ShouldUpdateFieldsAndReturnResponse()
        {
            var integration = new IntegrationDto { Id = 1 };
            var token = new TokenResponse { AccessToken = "at", RefreshToken = "rt" };
            var service = CreateService();

            _repo.Setup(r => r.UpdateAsync(It.IsAny<IntegrationDto>()))
                .ReturnsAsync((IntegrationDto i) => i);

            var response = await service.UpdateTokenAsync(integration, token);

            Assert.Equal("at", response.Result?.Token);
            Assert.Equal("rt", response.Result?.RefreshToken);
            _repo.Verify(r => r.UpdateAsync(It.IsAny<IntegrationDto>()), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateIntegrationAsync_WhenInvalid_ShouldReturnError()
        {
            var dto = new HubIntegracaoDto();
            _validator.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("field", "error") }));
            var service = CreateService();

            var response = await service.AddOrUpdateIntegrationAsync(dto);

            Assert.False(response.IsSuccess);
            Assert.NotNull(response.Error);
        }

        [Fact]
        public async Task AddOrUpdateIntegrationAsync_WhenExisting_ShouldUpdate()
        {
            var dto = new HubIntegracaoDto { Chave = "key" };
            var existing = new IntegrationDto { Id = 1 };
            _validator.Setup(v => v.ValidateAsync(dto, default)).ReturnsAsync(new ValidationResult());
            _repo.Setup(r => r.GetByKeyAsync(dto.Chave!)).ReturnsAsync(existing);
            _repo.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);
            var service = CreateService();

            var response = await service.AddOrUpdateIntegrationAsync(dto);

            Assert.True(response.IsSuccess);
            _repo.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task GetIntegrationByKeyAsync_ShouldReturnIntegration()
        {
            var integration = new IntegrationDto { HubKey = "key" };
            _repo.Setup(r => r.GetByKeyAsync("key")).ReturnsAsync(integration);
            var service = CreateService();

            var response = await service.GetIntegrationByKeyAsync("key");

            Assert.Same(integration, response.Result);
            _repo.Verify(r => r.GetByKeyAsync("key"), Times.Once);
        }
    }
}
