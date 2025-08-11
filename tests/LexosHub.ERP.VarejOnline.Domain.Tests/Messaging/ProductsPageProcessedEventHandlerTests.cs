using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class ProductsPageProcessedEventHandlerTests
    {
        private readonly Mock<ILogger<CriarProdutosSimplesEventHandler>> _logger = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly Mock<IIntegrationRepository> _integrationRepository = new();
        private readonly Mock<IOptions<SyncOutConfig>> _syncOutSqsConfigMock = new();
        private readonly Mock<ProdutoViewMapper> _produtoViewMapperMock = new();

        private CriarProdutosSimplesEventHandler CreateHandler() =>
            new CriarProdutosSimplesEventHandler(_logger.Object, _sqsRepository.Object, _syncOutSqsConfigMock.Object, _produtoViewMapperMock.Object);

        [Fact]
        public async Task HandleAsync_ShouldLogInformation()
        {
            var evt = new CriarProdutosSimples
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
