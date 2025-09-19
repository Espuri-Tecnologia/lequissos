using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class IntegrationCreatedEventHandlerTests
    {
        private readonly Mock<ILogger<IntegrationCreatedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private IntegrationCreatedEventHandler CreateHandler()
        {
            return new IntegrationCreatedEventHandler(_logger.Object, _integrationService.Object, _dispatcher.Object);
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

            _dispatcher.Verify(d => d.DispatchAsync(
                It.Is<InitialSync>(i => i.HubKey == evt.HubKey),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
