using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken);
    }
}
