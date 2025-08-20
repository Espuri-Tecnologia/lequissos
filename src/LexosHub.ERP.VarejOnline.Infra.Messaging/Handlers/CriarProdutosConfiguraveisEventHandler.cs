using Amazon.Auth.AccessControlPolicy;
using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.Hub.Sync.Models.Produto;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Linq;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;

public class CriarProdutosConfiguraveisEventHandler : IEventHandler<CriarProdutosConfiguraveis>
{
    private readonly ILogger<CriarProdutosConfiguraveisEventHandler> _logger;
    private readonly IIntegrationService _integrationService;
    private readonly IVarejOnlineApiService _apiService;
    private readonly ISqsRepository _syncOutSqsRepository;

    public CriarProdutosConfiguraveisEventHandler(
        ILogger<CriarProdutosConfiguraveisEventHandler> logger,
        IIntegrationService integrationService,
        IVarejOnlineApiService apiService,
        ISqsRepository syncOutSqsRepository,
        IOptions<SyncOutConfig> syncOutSqsConfig)
    {
        _logger = logger;
        _integrationService = integrationService;
        _apiService = apiService;
        _syncOutSqsRepository = syncOutSqsRepository;

        var syncOutConfig = syncOutSqsConfig.Value;
        _syncOutSqsRepository.IniciarFila($"{syncOutConfig.SQSBaseUrl}{syncOutConfig.SQSAccessKeyId}/{syncOutConfig.SQSName}");
    }

    public async Task HandleAsync(CriarProdutosConfiguraveis @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Produtos configuráveis recebidos para hub {HubKey}", @event.HubKey);

        var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
        var token = integrationResponse.Result?.Token ?? string.Empty;

        var produtosView = new List<ProdutoView>();

        if (@event.Produtos != null)
        {
            foreach (var produto in @event.Produtos)
            {
                
                //if (produtoView == null)
                //    continue;

                var request = new ProdutoRequest { ProdutoBase = produto.Id };
                var response = await _apiService.GetProdutosAsync(token, request);
                var variacoes = response.Result ?? new List<ProdutoResponse>();

                var produtoView = ProdutoConfiguravelViewMapper.Map(produto, variacoes);

                produtosView.Add(produtoView);
            }
        }

        if (produtosView.Any())
        {
            var json = JsonConvert.SerializeObject(
                produtosView,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ContractResolver = new IgnoreEmptyEnumerablesResolver()
                });

            var notificacao = new NotificacaoAtualizacaoModel
            {
                Chave = @event.HubKey,
                DataHora = DateTime.Now,
                Json = json,
                TipoProcesso = TipoProcessoAtualizacao.Produto,
                PlataformaId = 41
            };

            _syncOutSqsRepository.AdicionarMensagemFilaFifo(notificacao, $"notificacao-syncout-{notificacao.Chave}");
        }
    }
}

