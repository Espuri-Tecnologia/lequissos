using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class ProductsRequestedEventHandler : IEventHandler<ProductsRequested>
    {
        private readonly ILogger<ProductsRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejoOnlineApiService _apiService;
        private readonly IEventDispatcher _dispatcher;

        public ProductsRequestedEventHandler(
            ILogger<ProductsRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejoOnlineApiService apiService,
            IEventDispatcher dispatcher)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            _dispatcher = dispatcher;
        }

        public async Task HandleAsync(ProductsRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Products requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var start = @event.Inicio ?? 0;
            var pageSize = @event.Quantidade ?? 50;
            int count;

            do
            {
                var request = new ProdutoRequest
                {
                    Id = @event.Id,
                    Inicio = start,
                    Quantidade = pageSize,
                    AlteradoApos = @event.AlteradoApos,
                    Categoria = @event.Categoria,
                    ProdutoBase = @event.ProdutoBase,
                    Descricao = @event.Descricao,
                    CodigoBarras = @event.CodigoBarras,
                    CodigoInterno = @event.CodigoInterno,
                    CodigoSistema = @event.CodigoSistema,
                    SomenteAtivos = @event.SomenteAtivos,
                    SomenteComFotos = @event.SomenteComFotos,
                    SomenteEcommerce = @event.SomenteEcommerce,
                    SomenteMarketplace = @event.SomenteMarketplace,
                    AmostraGratis = @event.AmostraGratis,
                    AlteracaoDesde = @event.AlteracaoDesde,
                    AlteracaoAte = @event.AlteracaoAte,
                    CriacaoDesde = @event.CriacaoDesde,
                    CriacaoAte = @event.CriacaoAte,
                    IdsProdutos = @event.IdsProdutos,
                    IdsTabelasPrecos = @event.IdsTabelasPrecos
                };

                var response = await _apiService.GetProdutosAsync(token, request);
                count = response.Result?.Count ?? 0;

                var pageEvent = new ProductsPageProcessed
                {
                    HubKey = @event.HubKey,
                    Start = start,
                    PageSize = pageSize,
                    ProcessedCount = count
                };

                await _dispatcher.DispatchAsync(pageEvent, cancellationToken);

                start += pageSize;
            } while (count >= pageSize);
        }
    }
}

