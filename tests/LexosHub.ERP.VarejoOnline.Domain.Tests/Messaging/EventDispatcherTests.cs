using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class EventDispatcherTests
    {
        [Fact]
        public async Task DispatchAsync_ShouldInvokeRegisteredHandler()
        {
            var handlerMock = new Mock<IEventHandler<IntegrationCreated>>();
            var services = new ServiceCollection();
            services.AddSingleton<IEventHandler<IntegrationCreated>>(handlerMock.Object);
            using var provider = services.BuildServiceProvider();
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var dispatcher = new EventDispatcher(scopeFactory);

            var evt = new IntegrationCreated();
            await dispatcher.DispatchAsync(evt, CancellationToken.None);

            handlerMock.Verify(h => h.HandleAsync(evt, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
