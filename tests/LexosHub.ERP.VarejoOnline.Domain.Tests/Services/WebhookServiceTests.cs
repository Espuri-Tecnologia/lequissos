using System.Threading.Tasks;
using Moq;
using Xunit;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.Services;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Services
{
    public class WebhookServiceTests
    {
        private readonly Mock<IWebhookRepository> _repo = new();

        private WebhookService CreateService() => new WebhookService(_repo.Object);

        [Fact]
        public async Task AddAsync_ShouldReturnResponseWithWebhook()
        {
            var dto = new WebhookRecordDto { IntegrationId = 1, Url = "u", Method = "POST", Event = "E", Uuid = "id" };
            _repo.Setup(r => r.AddAsync(It.IsAny<WebhookRecordDto>()))
                .Callback<WebhookRecordDto>(w => w.Id = 5)
                .ReturnsAsync((WebhookRecordDto w) => w);

            var service = CreateService();
            var response = await service.AddAsync(dto);

            Assert.True(response.IsSuccess);
            Assert.Equal(5, response.Result?.Id);
            _repo.Verify(r => r.AddAsync(It.IsAny<WebhookRecordDto>()), Times.Once);
        }
    }
}
