using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class InitialSyncEventHandlerTests
    {
        private readonly Mock<ILogger<InitialSyncEventHandler>> _logger = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private InitialSyncEventHandler CreateHandler() =>
            new InitialSyncEventHandler(_logger.Object, _dispatcher.Object);

        [Fact]
        public async Task HandleAsync_ShouldDispatchCompaniesRequested()
        {
            var evt = new InitialSync { HubKey = "key" };

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<CompaniesRequested>(c => c.HubKey == evt.HubKey),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
