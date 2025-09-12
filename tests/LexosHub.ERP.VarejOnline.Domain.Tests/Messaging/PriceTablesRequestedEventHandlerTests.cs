using Amazon.SQS.Model;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Requests.Produto;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
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
        private readonly Mock<IOptions<VarejOnlineSqsConfig>> _sqsConfig = new();
        private readonly Mock<ISqsRepository> _sqs = new();

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
            var dispatcher = new SqslEventPublisher(_sqs.Object, _sqsConfig.Object);
            var options = Options.Create(new SyncOutConfig());
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
        public async Task HandleAsync_ShouldDispatchProductsRequestedWithIds()
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

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _sqs.Verify(s => s.AdicionarMensagemFilaNormal(
                    It.Is<SendMessageRequest>(r => IsProductsRequestedWithIds(r, "key", "1,2,3"))),
                Times.Once);
        }

        private bool IsProductsRequestedWithIds(SendMessageRequest request, string hubKey, string ids)
        {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(
                request.MessageBody,
                new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }
            );
            if (baseEvent is ProductsRequested p)
                return p.HubKey == hubKey && p.IdsTabelasPrecos == ids;
            return false;
        }
    }
}
