using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Estoque;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class StocksRequestedEventHandler : IEventHandler<StocksRequested>
    {
        private readonly ILogger<StocksRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly ISqsRepository _syncOutSqsRepository;
        private readonly int _defaultPageSize;

        public StocksRequestedEventHandler(
            ILogger<StocksRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncOutConfig> syncOutConfig,
            IConfiguration configuration)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            _syncOutSqsRepository = syncOutSqsRepository;

            var syncOut = syncOutConfig?.Value ?? throw new ArgumentNullException(nameof(syncOutConfig));
            _syncOutSqsRepository.IniciarFila($"{syncOut.SQSBaseUrl}{syncOut.SQSAccessKeyId}/{syncOut.SQSName}");

            _defaultPageSize = configuration.GetValue<int>("VarejOnlineApiSettings:DefaultPageSize");
        }

        public async Task HandleAsync(StocksRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stocks requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integrationResponse.Result?.Token ?? string.Empty;

            var start = @event.Inicio ?? 0;
            var pageSize = (@event.Quantidade.HasValue && @event.Quantidade.Value > 0)
                ? @event.Quantidade.Value
                : _defaultPageSize;

            int count;

            do
            {
                var request = new EstoqueRequest
                {
                    Produtos = @event.Produtos,
                    Entidades = @event.Entidades,
                    Inicio = start,
                    Quantidade = pageSize,
                    AlteradoApos = @event.AlteradoApos,
                    Data = @event.Data,
                    SomenteEcommerce = @event.SomenteEcommerce,
                    SomenteMarketplace = @event.SomenteMarketplace
                };

                var response = await _apiService.GetEstoquesAsync(token, request);
                var estoques = response.Result ?? new List<EstoqueResponse>();
                count = estoques.Count;

                if (count > 0)
                {
                    var mapped = EstoqueViewMapper.Map(estoques);

                    if (mapped.Count > 0)
                    {
                        var notification = new NotificacaoAtualizacaoModel
                        {
                            Chave = @event.HubKey,
                            DataHora = DateTime.Now,
                            Json = JsonConvert.SerializeObject(mapped),
                            TipoProcesso = TipoProcessoAtualizacao.Estoque,
                            PlataformaId = 41
                        };

                        _syncOutSqsRepository.AdicionarMensagemFilaFifo(notification, $"notificacao-syncout-{notification.Chave}");
                    }
                }
                else
                {
                    _logger.LogInformation("Nenhum estoque retornado para hub {HubKey} na página iniciando em {Start}", @event.HubKey, start);
                }

                start += pageSize;
            }
            while (count >= pageSize && !cancellationToken.IsCancellationRequested);
        }
    }
}