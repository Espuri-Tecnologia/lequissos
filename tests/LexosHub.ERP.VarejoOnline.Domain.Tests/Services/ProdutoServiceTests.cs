using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Services
{
    public class ProdutoServiceTests
    {
        private readonly Mock<ILogger<ProdutoService>> _logger = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private ProdutoService CreateService() => new ProdutoService(_logger.Object, _dispatcher.Object);

        [Fact]
        public async Task ProcessWebhookAsync_ShouldDispatchEvent()
        {
            var dto = new ProdutoWebhookDto
            {
                HubKey = "key",
                Produtos = new List<ProdutoResponse> { new() }
            };

            await CreateService().ProcessWebhookAsync(dto, CancellationToken.None);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<ProductsPageProcessed>(p => p.HubKey == "key" && p.ProcessedCount == 1),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
