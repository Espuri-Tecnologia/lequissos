using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers
{
    public class ProductsRequestedEventHandler : IEventHandler<ProductsRequested>
    {
        private readonly ILogger<ProductsRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejoOnlineApiService _apiService;

        public ProductsRequestedEventHandler(
            ILogger<ProductsRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejoOnlineApiService apiService)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task HandleAsync(ProductsRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Products requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var request = new ProdutoRequest
            {
                Id = @event.Id,
                Inicio = @event.Inicio,
                Quantidade = @event.Quantidade,
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

            await _apiService.GetProdutosAsync(token, request);
        }
    }
}
