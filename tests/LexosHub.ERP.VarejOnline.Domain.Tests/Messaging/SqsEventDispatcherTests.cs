using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
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
            var sqsMock = new Mock<IAmazonSQS>();
            var inMemory = new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:IntegrationCreated", "queue/test"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var dispatcher = new SqsEventDispatcher(sqsMock.Object, configuration);
            var evt = new IntegrationCreated { HubKey = "key" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            sqsMock.Verify(s => s.SendMessageAsync(
                It.Is<SendMessageRequest>(r =>
                    r.QueueUrl == "http://localhost/queue/test" &&
                    JsonSerializer.Deserialize<BaseEvent>(r.MessageBody, new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }) is IntegrationCreated),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DispatchAsync_ShouldSendCriarProdutosKitsToConfiguredQueue()
        {
            var sqsMock = new Mock<IAmazonSQS>();
            var inMemory = new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:ProdutosKits", "queue/produtokit"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var dispatcher = new SqsEventDispatcher(sqsMock.Object, configuration);
            var evt = new CriarProdutosKits { HubKey = "key" };

            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            sqsMock.Verify(s => s.SendMessageAsync(
                It.Is<SendMessageRequest>(r =>
                    r.QueueUrl == "http://localhost/queue/produtokit" &&
                    JsonSerializer.Deserialize<BaseEvent>(r.MessageBody, new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }) is CriarProdutosKits),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
