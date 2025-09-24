using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class InvoicesRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<InvoicesRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly IOptions<SyncOutConfig> _syncOutOptions = Options.Create(new SyncOutConfig
        {
            SQSBaseUrl = "https://sqs/",
            SQSAccessKeyId = "account",
            SQSName = "queue.fifo"
        });

        private InvoicesRequestedEventHandler CreateHandler() =>
            new(_logger.Object, _integrationService.Object, _apiService.Object, _sqsRepository.Object, _syncOutOptions);

        [Fact]
        public async Task HandleAsync_ShouldPublishInvoiceXmlNotification()
        {
            var integration = new IntegrationDto { Token = "token" };

            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            _apiService.Setup(s => s.GetInvoiceXmlAsync("token", 123, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response<string>("<xml>nota</xml>"));

            string? startedQueue = null;
            _sqsRepository.Setup(r => r.IniciarFila(It.IsAny<string>()))
                .Callback<string>(url => startedQueue = url);

            NotificacaoAtualizacaoModel? publishedNotification = null;
            string? publishedGroupId = null;
            _sqsRepository.Setup(r => r.AdicionarMensagemFilaFifo(It.IsAny<NotificacaoAtualizacaoModel>(), It.IsAny<string>()))
                .Callback<NotificacaoAtualizacaoModel, string>((notification, groupId) =>
                {
                    publishedNotification = notification;
                    publishedGroupId = groupId;
                });

            await CreateHandler().HandleAsync(new InvoicesRequested
            {
                HubKey = "hub",
                Number = 123
            }, CancellationToken.None);

            Assert.Equal("https://sqs/account/queue.fifo", startedQueue);
            Assert.NotNull(publishedNotification);
            Assert.Equal("hub", publishedNotification!.Chave);
            Assert.Equal("<xml>nota</xml>", publishedNotification.Json);
            Assert.Equal(TipoProcessoAtualizacao.NotaFiscal, publishedNotification.TipoProcesso);
            Assert.Equal((short)41, publishedNotification.PlataformaId);
            Assert.Equal("notificacao-syncout-hub", publishedGroupId);

            _apiService.Verify(s => s.GetInvoiceXmlAsync("token", 123, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldUseMockXmlWhenApiFails()
        {
            var integration = new IntegrationDto { Token = "token" };

            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            _apiService.Setup(s => s.GetInvoiceXmlAsync("token", 456, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response<string> { Error = new ErrorResult("fail") });

            NotificacaoAtualizacaoModel? publishedNotification = null;
            _sqsRepository.Setup(r => r.AdicionarMensagemFilaFifo(It.IsAny<NotificacaoAtualizacaoModel>(), It.IsAny<string>()))
                .Callback<NotificacaoAtualizacaoModel, string>((notification, _) => publishedNotification = notification);

            await CreateHandler().HandleAsync(new InvoicesRequested
            {
                HubKey = "hub",
                Number = 456
            }, CancellationToken.None);

            Assert.NotNull(publishedNotification);
            Assert.Contains("<nNF>456</nNF>", publishedNotification!.Json);
            Assert.Equal(TipoProcessoAtualizacao.NotaFiscal, publishedNotification.TipoProcesso);
        }
    }
}
