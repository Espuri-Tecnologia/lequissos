using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
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
                It.Is<SendMessageRequest>(r => IsCompaniesRequestedWithHubKey(r, evt.HubKey)),
                It.IsAny<CancellationToken>()), Times.Once);

        }
        private bool IsCompaniesRequestedWithHubKey(SendMessageRequest request, string hubKey)
        {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(
                request.MessageBody,
                new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }
            );
            if (baseEvent is CompaniesRequested c)
                return c.HubKey == hubKey;

            return false;
        }

    }
}
