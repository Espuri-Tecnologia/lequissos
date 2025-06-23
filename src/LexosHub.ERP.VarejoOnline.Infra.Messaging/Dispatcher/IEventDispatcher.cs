using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken);
    }
}
