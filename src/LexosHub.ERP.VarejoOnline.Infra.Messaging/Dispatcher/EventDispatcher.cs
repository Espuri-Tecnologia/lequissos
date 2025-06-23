using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using System.Reflection;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken)
        {
            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handler = _serviceProvider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"Handler para {@event.GetType().Name} não encontrado");

            var method = handlerType.GetMethod("HandleAsync", BindingFlags.Instance | BindingFlags.Public);
            await (Task)method.Invoke(handler, new object[] { @event, cancellationToken })!;
        }
    }
}
