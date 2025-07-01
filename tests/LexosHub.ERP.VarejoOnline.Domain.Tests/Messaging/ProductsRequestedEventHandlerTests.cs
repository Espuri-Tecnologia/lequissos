using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class ProductsRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<ProductsRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejoOnlineApiService> _apiService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();
        private readonly Mock<IConfiguration> _configuration = new();

        private ProductsRequestedEventHandler CreateHandler() =>
            new ProductsRequestedEventHandler(_logger.Object, _integrationService.Object, _apiService.Object, _dispatcher.Object, _configuration.Object);

        [Fact]
        public async Task HandleAsync_ShouldFetchIntegrationAndCallApiService()
        {
            var evt = new ProductsRequested
            {
                HubKey = "key",
                Id = 1,
                Quantidade = 5
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _integrationService.Verify(s => s.GetIntegrationByKeyAsync("key"), Times.Once);
            _apiService.Verify(a => a.GetProdutosAsync(
                    "token",
                    It.Is<ProdutoRequest>(r => r.Id == evt.Id && r.Quantidade == evt.Quantidade)
                ), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldProcessAllPagesAndDispatchEvents()
        {
            var evt = new ProductsRequested
            {
                HubKey = "key",
                Quantidade = 2
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var firstPage = new List<ProdutoResponse> { new(), new() };
            var secondPage = new List<ProdutoResponse> { new() };

            _apiService.SetupSequence(a => a.GetProdutosAsync("token", It.IsAny<ProdutoRequest>()))
                .ReturnsAsync(new Response<List<ProdutoResponse>>(firstPage))
                .ReturnsAsync(new Response<List<ProdutoResponse>>(secondPage));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _apiService.Verify(a => a.GetProdutosAsync(
                    "token",
                    It.Is<ProdutoRequest>(r => r.Inicio == 0 && r.Quantidade == 2)
                ), Times.Once);

            _apiService.Verify(a => a.GetProdutosAsync(
                    "token",
                    It.Is<ProdutoRequest>(r => r.Inicio == 2 && r.Quantidade == 2)
                ), Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<ProductsPageProcessed>(p => p.Start == 0 && p.PageSize == 2 && p.ProcessedCount == 2 && p.HubKey == "key"),
                    It.IsAny<CancellationToken>()), Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<ProductsPageProcessed>(p => p.Start == 2 && p.PageSize == 2 && p.ProcessedCount == 1 && p.HubKey == "key"),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

