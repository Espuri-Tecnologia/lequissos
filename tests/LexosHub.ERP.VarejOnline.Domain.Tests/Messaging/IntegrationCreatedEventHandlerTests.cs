using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using Amazon.SQS.Model;
using System.Text.Json;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class IntegrationCreatedEventHandlerTests
    {
        private readonly Mock<ILogger<IntegrationCreatedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IAmazonSQS> _sqs = new();
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:InitialSync", "queue/init"}
            })
            .Build();

        private IntegrationCreatedEventHandler CreateHandler()
        {
            var dispatcher = new SqsEventDispatcher(_sqs.Object, _configuration);
            return new IntegrationCreatedEventHandler(_logger.Object, _integrationService.Object, dispatcher);
        }

        [Fact]
        public async Task HandleAsync_ShouldCallAddOrUpdateIntegration()
        {
            var evt = new IntegrationCreated
            {
                HubIntegrationId = 1,
                TenantId = 2,
                HubKey = "key",
                Cnpj = "123"
            };

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _integrationService.Verify(s => s.AddOrUpdateIntegrationAsync(
                It.Is<HubIntegracaoDto>(d =>
                    d.IntegracaoId == evt.HubIntegrationId &&
                    d.TenantId == evt.TenantId &&
                    d.Chave == evt.HubKey &&
                    d.Cnpj == evt.Cnpj &&
                    d.Habilitado &&
                    d.Excluido == false
                )), Times.Once);

            _sqs.Verify(s => s.SendMessageAsync(
                It.Is<SendMessageRequest>(r => IsInitialSyncWithHubKey(r, evt.HubKey)),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        private bool IsInitialSyncWithHubKey(SendMessageRequest request, string hubKey)
        {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(
                request.MessageBody,
                new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }
            );
            if (baseEvent is InitialSync i)
                return i.HubKey == hubKey;

            return false;
        }
    }
}
