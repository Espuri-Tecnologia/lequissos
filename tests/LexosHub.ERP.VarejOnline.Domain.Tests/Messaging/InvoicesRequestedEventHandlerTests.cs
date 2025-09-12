using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class InvoicesRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<InvoicesRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();

        private InvoicesRequestedEventHandler CreateHandler()
        {
            return new InvoicesRequestedEventHandler(_logger.Object, _integrationService.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldFetchIntegration()
        {
            var evt = new InvoicesRequested
            {
                HubKey = "hub",
                Number = 123
            };

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _integrationService.Verify(s => s.GetIntegrationByKeyAsync("hub"), Times.Once);
        }
    }
}
