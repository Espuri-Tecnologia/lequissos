using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;

        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Pedido criado: {@event.OrderId}, Cliente: {@event.CustomerName}");
            return Task.CompletedTask;
        }
    }
}
