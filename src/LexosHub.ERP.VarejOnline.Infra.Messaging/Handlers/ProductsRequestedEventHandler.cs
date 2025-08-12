using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class ProductsRequestedEventHandler : IEventHandler<ProductsRequested>
    {
        private readonly ILogger<ProductsRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly IEventDispatcher _dispatcher;
        private readonly int _defaultPageSize;

        public ProductsRequestedEventHandler(
            ILogger<ProductsRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            IEventDispatcher dispatcher,
            IConfiguration configuration)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            _dispatcher = dispatcher;
            _defaultPageSize = configuration.GetValue<int>("VarejOnlineApiSettings:DefaultPageSize");
        }

        public async Task HandleAsync(ProductsRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Products requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var start = @event.Inicio ?? 0;
            var pageSize = (@event.Quantidade.HasValue && @event.Quantidade.Value > 0)
                ? @event.Quantidade.Value
                : _defaultPageSize;

            var produtosConfiguraveis = new List<ProdutoResponse>();
            int totalKits = 0;
            int totalSimples = 0;
            int totalConfiguraveis = 0;

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
                var produtos = response.Result;
                count = produtos?.Count ?? 0;

                if (produtos == null || count == 0)
                {
                    _logger.LogInformation("Nenhum produto retornado. Encerrando processamento.");
                    break;
                }

                var kits = produtos.Where(p => p.Componentes != null && p.Componentes.Any()).ToList();
                var simples = produtos
                    .Where(p => (p.Componentes == null || !p.Componentes.Any()) && p.MercadoriaBase != true)
                    .ToList();
                var configuraveis = produtos
                    .Where(p => (p.Componentes == null || !p.Componentes.Any()) && p.MercadoriaBase == true)
                    .ToList();

                _logger.LogInformation(
                    "Página {Start} classificada: {SimplesCount} simples, {ConfiguraveisCount} configuráveis, {KitsCount} kits",
                    start,
                    simples.Count,
                    configuraveis.Count,
                    kits.Count);

                if (simples.Any())
                {
                    var pageEvent = new CriarProdutosSimples
                    {
                        HubKey = @event.HubKey,
                        Start = start,
                        PageSize = pageSize,
                        Produtos = simples,
                        ProcessedCount = simples.Count
                    };

                    await _dispatcher.DispatchAsync(pageEvent, cancellationToken);
                    totalSimples += simples.Count;
                }

                if (kits.Any())
                {
                    var kitsEvent = new CriarProdutosKits
                    {
                        HubKey = @event.HubKey,
                        Start = start,
                        PageSize = pageSize,
                        Produtos = kits,
                        ProcessedCount = kits.Count
                    };

                    await _dispatcher.DispatchAsync(kitsEvent, cancellationToken);
                    totalKits += kits.Count;
                }

                produtosConfiguraveis.AddRange(configuraveis);
                totalConfiguraveis += configuraveis.Count;

                start += pageSize;

            } while (count >= pageSize);

            if (produtosConfiguraveis.Any())
            {
                var configuraveisEvent = new CriarProdutosConfiguraveis
                {
                    HubKey = @event.HubKey,
                    Produtos = produtosConfiguraveis,
                    ProcessedCount = produtosConfiguraveis.Count
                };
                await _dispatcher.DispatchAsync(configuraveisEvent, cancellationToken);
            }

            _logger.LogInformation(
                "Processamento finalizado. Simples: {Simples}, Configuráveis: {Configuraveis}, Kits: {Kits}",
                totalSimples,
                totalConfiguraveis,
                totalKits);
        }

    }
}

