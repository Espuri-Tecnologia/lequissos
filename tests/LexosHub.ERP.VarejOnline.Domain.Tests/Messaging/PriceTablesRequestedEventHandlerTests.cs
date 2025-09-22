using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Requests.Produto;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class PriceTablesRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<PriceTablesRequestedEventHandler>> _logger = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private readonly SyncOutConfig _syncOutConfig = new()
        {
            SQSBaseUrl = "https://sqs.local/",
            SQSAccessKeyId = "access",
            SQSName = "queue"
        };

        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:PriceTablePageProcessed", "queue/pricetable"},
                {"AWS:SQSQueues:ProductsRequested", "queue/products"},
                {"VarejOnlineApiSettings:DefaultPageSize", "2"}
            })
            .Build();

        private PriceTablesRequestedEventHandler CreateHandler()
        {
            var dispatcher = _dispatcher.Object;
            var options = Options.Create(_syncOutConfig);
            return new PriceTablesRequestedEventHandler(
                _logger.Object,
                _sqsRepository.Object,
                options,
                _apiService.Object,
                _integrationService.Object,
                _configuration,
                dispatcher);
        }

        [Fact]
        public async Task HandleAsync_ShouldDispatchPriceTablePageProcessedForEachPage()
        {
            var evt = new PriceTablesRequested { HubKey = "key" };
            var integration = new IntegrationDto { Token = "token" };

            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var firstPage = new List<TabelaPrecoListResponse>
            {
                new TabelaPrecoListResponse { Id = 1 },
                new TabelaPrecoListResponse { Id = 2 }
            };

            var secondPage = new List<TabelaPrecoListResponse>
            {
                new TabelaPrecoListResponse { Id = 3 }
            };

            _apiService.SetupSequence(a => a.GetPriceTablesAsync("token", It.IsAny<TabelaPrecoRequest>()))
                .ReturnsAsync(new Response<List<TabelaPrecoListResponse>>(firstPage))
                .ReturnsAsync(new Response<List<TabelaPrecoListResponse>>(secondPage));

            var dispatchedEvents = new List<PriceTablePageProcessed>();

            _dispatcher
                .Setup(d => d.DispatchAsync(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()))
                .Callback<BaseEvent, CancellationToken>((evt, _) =>
                {
                    if (evt is PriceTablePageProcessed processed)
                    {
                        dispatchedEvents.Add(processed);
                    }
                })
                .Returns(Task.CompletedTask);

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            Assert.Collection(
                dispatchedEvents,
                e =>
                {
                    Assert.Equal("key", e.HubKey);
                    Assert.Equal(0, e.Start);
                    Assert.Equal(2, e.PageSize);
                    Assert.Equal(firstPage, e.PriceTables);
                    Assert.Equal(firstPage.Count, e.ProcessedCount);
                },
                e =>
                {
                    Assert.Equal("key", e.HubKey);
                    Assert.Equal(2, e.Start);
                    Assert.Equal(2, e.PageSize);
                    Assert.Equal(secondPage, e.PriceTables);
                    Assert.Equal(secondPage.Count, e.ProcessedCount);
                });

            _sqsRepository.Verify(s => s.IniciarFila("https://sqs.local/access/queue"), Times.Once);
        }
    }
}
