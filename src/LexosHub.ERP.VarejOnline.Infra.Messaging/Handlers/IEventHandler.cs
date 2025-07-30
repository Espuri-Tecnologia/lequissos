using System.Threading;
using System.Threading.Tasks;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public interface IEventHandler<in T> where T : BaseEvent
    {
        Task HandleAsync(T @event, CancellationToken cancellationToken);
    }
}
