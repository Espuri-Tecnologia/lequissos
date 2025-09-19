using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class SqsEventDispatcherTests
    {
        [Fact]
        public async Task DispatchAsync_ShouldSendSerializedEventToQueue()
        {
            var sqsRepository = new Mock<ISqsRepository>();
            var sqsOptions = Options.Create(new VarejOnlineSqsConfig
            {
                SQSBaseUrl = "http://localhost/",
                SQSAccessKeyId = "123456789",
                SQSQueues = new Dictionary<string, string>
                {
                    { "IntegrationCreated", "queue/test" }
                }
            });

            var dispatcher = new SqslEventPublisher(sqsRepository.Object, sqsOptions);
            var evt = new IntegrationCreated { HubKey = "key" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            var expectedQueueUrl = "http://localhost/123456789/queue/test";

            sqsRepository.Verify(s => s.IniciarFila(expectedQueueUrl, Lexos.SQS.Models.TipoConta.Production), Times.Once);
            sqsRepository.Verify(s => s.AdicionarMensagemFilaNormal(
                It.Is<BaseEvent>(message => ReferenceEquals(message, evt))), Times.Once);
            sqsRepository.Verify(s => s.AdicionarMensagemFilaFifo(It.IsAny<BaseEvent>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DispatchAsync_ShouldSendCriarProdutosKitsToConfiguredQueue()
        {
            var sqsRepository = new Mock<ISqsRepository>();
            var sqsOptions = Options.Create(new VarejOnlineSqsConfig
            {
                SQSBaseUrl = "http://localhost/",
                SQSAccessKeyId = "123456789",
                SQSQueues = new Dictionary<string, string>
                {
                    { "Produtos", "queue/produtokit" }
                }
            });

            var dispatcher = new SqslEventPublisher(sqsRepository.Object, sqsOptions);
            var evt = new CriarProdutosKits { HubKey = "key" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            var expectedQueueUrl = "http://localhost/123456789/queue/produtokit";

            sqsRepository.Verify(s => s.IniciarFila(expectedQueueUrl, Lexos.SQS.Models.TipoConta.Production), Times.Once);
            sqsRepository.Verify(s => s.AdicionarMensagemFilaNormal(
                It.Is<BaseEvent>(message => ReferenceEquals(message, evt))), Times.Once);
            sqsRepository.Verify(s => s.AdicionarMensagemFilaFifo(It.IsAny<BaseEvent>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DispatchAsync_ShouldHandleFifoQueues()
        {
            var sqsRepository = new Mock<ISqsRepository>();
            var sqsOptions = Options.Create(new VarejOnlineSqsConfig
            {
                SQSBaseUrl = "http://localhost/",
                SQSAccessKeyId = "123456789",
                SQSQueues = new Dictionary<string, string>
                {
                    { "Produtos", "queue/produtos.fifo" }
                }
            });

            var dispatcher = new SqslEventPublisher(sqsRepository.Object, sqsOptions);
            var evt = new CriarProdutosSimples { HubKey = "fifo-hub" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            var expectedQueueUrl = "http://localhost/123456789/queue/produtos.fifo";

            sqsRepository.Verify(s => s.IniciarFila(expectedQueueUrl, Lexos.SQS.Models.TipoConta.Production), Times.Once);
            sqsRepository.Verify(s => s.AdicionarMensagemFilaFifo(
                It.Is<BaseEvent>(message => ReferenceEquals(message, evt)),
                It.Is<string>(group => group == evt.HubKey)), Times.Once);
            sqsRepository.Verify(s => s.AdicionarMensagemFilaNormal(It.IsAny<BaseEvent>()), Times.Never);
        }
    }
}
