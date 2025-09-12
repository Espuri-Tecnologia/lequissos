using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public interface ISqslEventPublisher
    {
        Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken);
    }
}
