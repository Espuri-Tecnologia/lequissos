using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class ProductsRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<ProductsRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejoOnlineApiService> _apiService = new();
        private readonly Mock<IAmazonSQS> _sqs = new();
        private readonly Mock<IConfiguration> _configuration = new();

        private ProductsRequestedEventHandler CreateHandler()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"AWS:ServiceURL", "http://localhost"},
                    {"AWS:SQSQueues:ProductsPageProcessed", "queue/page"}
                })
                .Build();
            var dispatcher = new SqsEventDispatcher(_sqs.Object, config);
            return new ProductsRequestedEventHandler(_logger.Object, _integrationService.Object, _apiService.Object, dispatcher, _configuration.Object);
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

            _sqs.Verify(s => s.SendMessageAsync(
                    It.Is<SendMessageRequest>(r =>
                        IsProductsPageProcessed(r, 0, 2, 2, "key", firstPage)),
                    It.IsAny<CancellationToken>()), Times.Once);

            _sqs.Verify(s => s.SendMessageAsync(
                    It.Is<SendMessageRequest>(r =>
                        IsProductsPageProcessed(r, 2, 2, 1, "key", secondPage)),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        private bool IsProductsPageProcessed(
            SendMessageRequest request,
            int expectedStart,
            int expectedPageSize,
            int expectedProcessedCount,
            string expectedHubKey,
            List<ProdutoResponse> expectedProdutos)
        {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(
                request.MessageBody,
                new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }
            );
            if (baseEvent is ProductsPageProcessed p)
            {
                return p.Start == expectedStart
                    && p.PageSize == expectedPageSize
                    && p.ProcessedCount == expectedProcessedCount
                    && p.HubKey == expectedHubKey
                    && p.Produtos is IEnumerable<ProdutoResponse> produtos
                    && ProdutosAreEqual(produtos, expectedProdutos);
            }
            return false;
        }

        private bool ProdutosAreEqual(IEnumerable<ProdutoResponse> a, IEnumerable<ProdutoResponse> b)
        {
            var aList = a.ToList();
            var bList = b.ToList();
            if (aList.Count != bList.Count)
                return false;

            for (int i = 0; i < aList.Count; i++)
            {
                if (aList[i].Id != bList[i].Id)
                    return false;
            }
            return true;
        }
    }
}
