using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class ProductsPageProcessedEventHandler : IEventHandler<ProductsPageProcessed>
    {
        private readonly ILogger<ProductsPageProcessedEventHandler> _logger;

        public ProductsPageProcessedEventHandler(ILogger<ProductsPageProcessedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(ProductsPageProcessed @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Pgina processada recebida. Hub: {HubKey}, Incio: {Start}, Quantidade: {PageSize}, Processados: {ProcessedCount}, Produtos: {ProductsCount}",
                @event.HubKey,
                @event.Start,
                @event.PageSize,
                @event.ProcessedCount,
                @event.Produtos?.Count ?? 0);

            return Task.CompletedTask;
        }
    }
}
