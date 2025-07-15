using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Domain.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly ILogger<ProdutoService> _logger;
        private readonly IEventDispatcher _dispatcher;

        public ProdutoService(ILogger<ProdutoService> logger, IEventDispatcher dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public async Task ProcessWebhookAsync(ProdutoWebhookDto produto, CancellationToken cancellationToken = default)
        {
            if (produto == null)
                throw new ArgumentNullException(nameof(produto));

            _logger.LogInformation("Produto recebido para o hub {HubKey} com {Count} produtos", produto.HubKey, produto.Produtos?.Count ?? 0);

            var evt = new ProductsPageProcessed
            {
                HubKey = produto.HubKey,
                Start = 0,
                PageSize = produto.Produtos?.Count ?? 0,
                ProcessedCount = produto.Produtos?.Count ?? 0,
                Produtos = produto.Produtos
            };

            await _dispatcher.DispatchAsync(evt, cancellationToken);
        }
    }
}
