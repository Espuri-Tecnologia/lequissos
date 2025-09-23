using Amazon.SQS;
using Amazon.SQS.Model;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
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
            var _sqsConfig = new Mock<IOptions<VarejOnlineSqsConfig>>();
            var sqsRepository = new Mock<ISqsRepository>();
            var inMemory = new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:IntegrationCreated", "queue/test"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var dispatcher = new SqslEventPublisher(sqsRepository.Object, _sqsConfig.Object);
            var evt = new IntegrationCreated { HubKey = "key" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            sqsRepository.Verify(s => s.AdicionarMensagemFilaNormal(
                It.Is<SendMessageRequest>(r =>
                    r.QueueUrl == "http://localhost/queue/test" &&
                    JsonSerializer.Deserialize<BaseEvent>(r.MessageBody, new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }) is IntegrationCreated)), Times.Once);
        }

        [Fact]
        public async Task DispatchAsync_ShouldSendCriarProdutosKitsToConfiguredQueue()
        {
            var _sqsConfig = new Mock<IOptions<VarejOnlineSqsConfig>>();
            var sqsRepository = new Mock<ISqsRepository>();
            var inMemory = new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:ProdutosKits", "queue/produtokit"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var dispatcher = new SqslEventPublisher(sqsRepository.Object, _sqsConfig.Object);
            var evt = new CriarProdutosKits { HubKey = "key" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            sqsRepository.Verify(s => s.AdicionarMensagemFilaNormal(
                It.Is<SendMessageRequest>(r =>
                    r.QueueUrl == "http://localhost/queue/produtokit" &&
                    JsonSerializer.Deserialize<BaseEvent>(r.MessageBody, new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }) is CriarProdutosKits)), Times.Once);
        }
    }
}
