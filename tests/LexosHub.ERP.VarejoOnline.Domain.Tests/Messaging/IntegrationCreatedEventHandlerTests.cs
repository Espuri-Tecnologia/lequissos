using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class IntegrationCreatedEventHandlerTests
    {
        private readonly Mock<ILogger<IntegrationCreatedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private IntegrationCreatedEventHandler CreateHandler()
            => new IntegrationCreatedEventHandler(_logger.Object, _integrationService.Object, _dispatcher.Object);

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
