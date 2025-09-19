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
    public class InitialSyncEventHandlerTests
    {
        private readonly Mock<ILogger<InitialSyncEventHandler>> _logger = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private InitialSyncEventHandler CreateHandler()
        {
            return new InitialSyncEventHandler(_logger.Object, _dispatcher.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldDispatchExpectedEvents()
        {
            var evt = new InitialSync { HubKey = "key" };
            var cancellationToken = CancellationToken.None;

            await CreateHandler().HandleAsync(evt, cancellationToken);

            _dispatcher.Verify(d => d.DispatchAsync(
                It.Is<StoresRequested>(e => e.HubKey == evt.HubKey),
                It.Is<CancellationToken>(ct => ct == cancellationToken)), Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                It.Is<RegisterDefaultWebhooks>(e => e.HubKey == evt.HubKey),
                It.Is<CancellationToken>(ct => ct == cancellationToken)), Times.Once);
        }

    }
}
