using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class ProductsRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<ProductsRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        public ProductsRequestedEventHandlerTests()
        {
            _dispatcher
                .Setup(d => d.DispatchAsync(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private ProductsRequestedEventHandler CreateHandler()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"VarejOnlineApiSettings:DefaultPageSize", "10"}
                })
                .Build();
            return new ProductsRequestedEventHandler(
                _logger.Object,
                _integrationService.Object,
                _apiService.Object,
                _dispatcher.Object,
                configuration);
        }

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
        public async Task HandleAsync_WithProdutoBase_ShouldCallApiServiceWithProdutoBase()
        {
            var evt = new ProductsRequested
            {
                HubKey = "key",
                ProdutoBase = 10
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _apiService.Verify(a => a.GetProdutosAsync(
                    "token",
                    It.Is<ProdutoRequest>(r => r.ProdutoBase == evt.ProdutoBase)
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

            var firstPage = new List<ProdutoResponse> { new ProdutoResponse { Id = 1 }, new ProdutoResponse { Id = 2 } };
            var secondPage = new List<ProdutoResponse> { new ProdutoResponse { Id = 3 } };

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
                    It.Is<BaseEvent>(e => IsCriarProdutosSimples(
                        e,
                        "key",
                        0,
                        2,
                        2,
                        firstPage)),
                    It.Is<CancellationToken>(c => c == CancellationToken.None)),
                Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => IsCriarProdutosSimples(
                        e,
                        "key",
                        2,
                        2,
                        1,
                        secondPage)),
                    It.Is<CancellationToken>(c => c == CancellationToken.None)),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenKitsExist_ShouldDispatchKitsEventWithCorrectCount()
        {
            var evt = new ProductsRequested
            {
                HubKey = "key",
                Quantidade = 5
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var kit = new ProdutoResponse
            {
                Id = 1,
                Componentes = new List<ComponenteResponse> { new ComponenteResponse { Quantidade = 1 } }
            };

            var simples = new ProdutoResponse { Id = 2 };

            var page = new List<ProdutoResponse> { kit, simples };

            _apiService.Setup(a => a.GetProdutosAsync("token", It.IsAny<ProdutoRequest>()))
                .ReturnsAsync(new Response<List<ProdutoResponse>>(page));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => IsCriarProdutosSimples(
                        e,
                        "key",
                        0,
                        5,
                        1,
                        new List<ProdutoResponse> { simples })),
                    It.Is<CancellationToken>(c => c == CancellationToken.None)),
                Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => IsCriarProdutosKits(
                        e,
                        "key",
                        5,
                        5,
                        1,
                        new List<ProdutoResponse> { kit })),
                    It.Is<CancellationToken>(c => c == CancellationToken.None)),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenConfiguraveisExist_ShouldDispatchConfiguraveisEvent()
        {
            var evt = new ProductsRequested
            {
                HubKey = "key",
                Quantidade = 3
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var configuravel = new ProdutoResponse
            {
                Id = 10,
                MercadoriaBase = true
            };

            var page = new List<ProdutoResponse> { configuravel };

            _apiService.Setup(a => a.GetProdutosAsync("token", It.IsAny<ProdutoRequest>()))
                .ReturnsAsync(new Response<List<ProdutoResponse>>(page));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => IsCriarProdutosConfiguraveis(
                        e,
                        "key",
                        1,
                        new List<ProdutoResponse> { configuravel })),
                    It.Is<CancellationToken>(c => c == CancellationToken.None)),
                Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => e is CriarProdutosSimples),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => e is CriarProdutosKits),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static bool IsCriarProdutosSimples(
            BaseEvent @event,
            string expectedHubKey,
            int expectedStart,
            int expectedPageSize,
            int expectedProcessedCount,
            IEnumerable<ProdutoResponse> expectedProdutos) =>
            @event is CriarProdutosSimples produtosSimples
            && produtosSimples.HubKey == expectedHubKey
            && produtosSimples.Start == expectedStart
            && produtosSimples.PageSize == expectedPageSize
            && produtosSimples.ProcessedCount == expectedProcessedCount
            && ProdutosAreEqual(produtosSimples.Produtos, expectedProdutos);

        private static bool IsCriarProdutosKits(
            BaseEvent @event,
            string expectedHubKey,
            int expectedStart,
            int expectedPageSize,
            int expectedProcessedCount,
            IEnumerable<ProdutoResponse> expectedProdutos) =>
            @event is CriarProdutosKits produtosKits
            && produtosKits.HubKey == expectedHubKey
            && produtosKits.Start == expectedStart
            && produtosKits.PageSize == expectedPageSize
            && produtosKits.ProcessedCount == expectedProcessedCount
            && ProdutosAreEqual(produtosKits.Produtos, expectedProdutos);

        private static bool IsCriarProdutosConfiguraveis(
            BaseEvent @event,
            string expectedHubKey,
            int expectedProcessedCount,
            IEnumerable<ProdutoResponse> expectedProdutos) =>
            @event is CriarProdutosConfiguraveis produtosConfiguraveis
            && produtosConfiguraveis.HubKey == expectedHubKey
            && produtosConfiguraveis.ProcessedCount == expectedProcessedCount
            && ProdutosAreEqual(produtosConfiguraveis.Produtos, expectedProdutos);

        private static bool ProdutosAreEqual(
            IEnumerable<ProdutoResponse>? actual,
            IEnumerable<ProdutoResponse> expected)
        {
            if (actual is null)
            {
                return !expected.Any();
            }

            var actualList = actual.ToList();
            var expectedList = expected.ToList();

            if (actualList.Count != expectedList.Count)
            {
                return false;
            }

            return !actualList.Where((t, i) => t.Id != expectedList[i].Id).Any();
        }
    }
}
