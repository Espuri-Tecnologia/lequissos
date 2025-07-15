using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Amazon.SQS.Model;
using System.Text.Json;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Converters;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class InitialSyncEventHandlerTests
    {
        private readonly Mock<ILogger<InitialSyncEventHandler>> _logger = new();
        private readonly Mock<IAmazonSQS> _sqs = new();
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:CompaniesRequested", "queue/companies"}
            })
            .Build();

        private InitialSyncEventHandler CreateHandler()
        {
            var dispatcher = new SqsEventDispatcher(_sqs.Object, _configuration);
            return new InitialSyncEventHandler(_logger.Object, dispatcher);
        }

        [Fact]
        public async Task HandleAsync_ShouldDispatchCompaniesRequested()
        {
            var evt = new InitialSync { HubKey = "key" };

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _sqs.Verify(s => s.SendMessageAsync(
                    It.Is<SendMessageRequest>(r =>
                        r.QueueUrl == "http://localhost/queue/companies" &&
                        JsonSerializer.Deserialize<BaseEvent>(r.MessageBody, new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }) is CompaniesRequested c && c.HubKey == evt.HubKey),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
