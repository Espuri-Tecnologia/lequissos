using Lexos.SQS.Interface;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class PriceTablesPageProcessedEventHandlerTests
    {
        private readonly Mock<ILogger<PriceTablesRequestedEventHandler>> _logger = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly Mock<IIntegrationRepository> _integrationRepository = new();
        private readonly Mock<IOptions<SyncOutConfig>> _syncOutSqsConfigMock = new();
        private readonly Mock<IVarejoOnlineApiService> _varejoOnlineApiService = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();
        private readonly Mock<IConfiguration> _configuration = new();

        private PriceTablesRequestedEventHandler CreateHandler() =>
            new PriceTablesRequestedEventHandler(_logger.Object, _sqsRepository.Object, _syncOutSqsConfigMock.Object, _varejoOnlineApiService.Object, _integrationService.Object, _configuration.Object, _dispatcher.Object);

        [Fact]
        public async Task HandleAsync_ShouldLogInformation()
        {
            var evt = new PriceTablesRequested
            {
                HubKey = "key",
                Start = 1,
                PageSize = 2,
                ProcessedCount = 2,
                PriceTables = new List<TabelaPrecoListResponse> { new(), new() }
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
