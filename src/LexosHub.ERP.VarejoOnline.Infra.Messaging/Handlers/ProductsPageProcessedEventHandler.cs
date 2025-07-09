using Lexos.Hub.Sync.Models;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers.Produto;
using Microsoft.Extensions.Logging;
using System.Linq;

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

            if (@event.Produtos.Count > 0)
            {
                var mapped = @event.Produtos.Map();
            }

            return Task.CompletedTask;
        }
    }
}
