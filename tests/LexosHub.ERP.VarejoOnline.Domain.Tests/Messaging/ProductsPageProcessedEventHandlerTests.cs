using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class ProductsPageProcessedEventHandlerTests
    {
        private readonly Mock<ILogger<ProductsPageProcessedEventHandler>> _logger = new();

        private ProductsPageProcessedEventHandler CreateHandler() =>
            new ProductsPageProcessedEventHandler(_logger.Object);

        [Fact]
        public async Task HandleAsync_ShouldLogInformation()
        {
            var evt = new ProductsPageProcessed
            {
                HubKey = "key",
                Start = 1,
                PageSize = 2,
                ProcessedCount = 2,
                Produtos = new List<ProdutoResponse> { new(), new() }
            };

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _logger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("key")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
