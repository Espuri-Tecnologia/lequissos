using Amazon.SQS;
using Amazon.SQS.Model;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
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
    public class IntegrationCreatedEventHandlerTests
    {
        private readonly Mock<ILogger<IntegrationCreatedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IOptions<VarejOnlineSqsConfig>> _sqsConfig = new();
        private readonly Mock<ISqsRepository> _sqs = new();

        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:InitialSync", "queue/init"}
            })
            .Build();

        private IntegrationCreatedEventHandler CreateHandler()
        {
            var dispatcher = new SqslEventPublisher(_sqs.Object, _sqsConfig.Object);
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

            _sqs.Verify(s => s.AdicionarMensagemFilaNormal(
                It.Is<SendMessageRequest>(r => IsInitialSyncWithHubKey(r, evt.HubKey))), Times.Once);
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
