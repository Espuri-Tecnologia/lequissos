using System.Threading;
using System.Threading.Tasks;
using LexosHub.ERP.Winthor.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public interface IEventHandler<in T> where T : BaseEvent
    {
        Task HandleAsync(T @event, CancellationToken cancellationToken);
    }
}
