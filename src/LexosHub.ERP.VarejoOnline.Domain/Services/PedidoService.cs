using LexosHub.ERP.VarejoOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly ILogger<PedidoService> _logger;
        private readonly IEventDispatcher _dispatcher;

        public PedidoService(ILogger<PedidoService> logger, IEventDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task ProcessWebhookAsync(PedidoWebhookDto pedido, CancellationToken cancellationToken = default)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            _logger.LogInformation("Pedido recebido para o hub {HubKey} com {Count} produtos", pedido.HubKey, pedido.Produtos?.Count ?? 0);

            var evt = new ProductsPageProcessed
            {
                HubKey = pedido.HubKey,
                Start = 0,
                PageSize = pedido.Produtos?.Count ?? 0,
                ProcessedCount = pedido.Produtos?.Count ?? 0,
                Produtos = pedido.Produtos
            };

            await _dispatcher.DispatchAsync(evt, cancellationToken);
        }
    }
}
