using System.Threading.Tasks;
using System.Threading;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Webhook;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Webhook;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Services
{
    public class WebhookServiceTests
    {
        private readonly Mock<IWebhookRepository> _repo = new();
        private readonly Mock<IIntegrationService> _integration = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();

        private WebhookService CreateService() => new WebhookService(_repo.Object, _integration.Object, _apiService.Object);

        [Fact]
        public async Task AddAsync_ShouldReturnResponseWithWebhook()
        {
            var dto = new WebhookRecordDto { IntegrationId = 1, Url = "u", Types = new List<string> { "POST", "PUT" }, Event = "E", Uuid = "id" };
            _repo.Setup(r => r.AddAsync(It.IsAny<WebhookRecordDto>()))
                .Callback<WebhookRecordDto>(w =>
                {
                    Assert.True(w.Types.SequenceEqual(dto.Types));
                    w.Id = 5;
                })
                .ReturnsAsync((WebhookRecordDto w) => w);

            var service = CreateService();
            var response = await service.AddAsync(dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(5, response.Result?.Id);
            _repo.Verify(r => r.AddAsync(It.IsAny<WebhookRecordDto>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterAndPersistWebhook()
        {
            var dto = new LexosHub.ERP.VarejOnline.Domain.DTOs.Produto.WebhookDto { HubKey = "k", Event = "E", Types = new List<string> { "POST", "PUT" }, Url = "u" };
            var integration = new LexosHub.ERP.VarejOnline.Domain.DTOs.Integration.IntegrationDto { Id = 1, Token = "t" };
            var integrationResponse = new Response<LexosHub.ERP.VarejOnline.Domain.DTOs.Integration.IntegrationDto>(integration);
            _integration.Setup(i => i.GetIntegrationByKeyAsync("k")).ReturnsAsync(integrationResponse);

            var opResponse = new OperationResponse { IdRecurso = "uuid" };
            _apiService.Setup(a => a.RegisterWebhookAsync("t", It.Is<WebhookRequest>(r => r.types.SequenceEqual(dto.Types)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response<OperationResponse>(opResponse));

            _repo.Setup(r => r.AddAsync(It.IsAny<WebhookRecordDto>()))
                .Callback<WebhookRecordDto>(w =>
                {
                    Assert.True(w.Types.SequenceEqual(dto.Types));
                    w.Id = 10;
                })
                .ReturnsAsync((WebhookRecordDto w) => w);

            var service = CreateService();
            var response = await service.RegisterAsync(dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(10, response.Result?.Id);
            _integration.Verify(i => i.GetIntegrationByKeyAsync("k"), Times.Once);
            _apiService.Verify(a => a.RegisterWebhookAsync("t", It.IsAny<WebhookRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(r => r.AddAsync(It.IsAny<WebhookRecordDto>()), Times.Once);
        }
    }
}
