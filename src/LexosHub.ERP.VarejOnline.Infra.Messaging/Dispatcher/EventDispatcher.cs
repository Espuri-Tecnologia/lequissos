using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventDispatcher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handler = (dynamic)scope.ServiceProvider.GetRequiredService(handlerType);
            await handler.HandleAsync((dynamic)@event, cancellationToken);
        }
    }
}
