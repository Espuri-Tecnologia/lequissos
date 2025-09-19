using Amazon.Runtime.Internal;
using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.Hub.Sync.Models.Pedido;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreated>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly IEventDispatcher _dispatcher;
        private readonly ISqsRepository _syncInSqsRepository;

        public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger, 
            IEventDispatcher dispatcher, 
            IIntegrationService integrationService, 
            IVarejOnlineApiService apiService, 
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncInConfig> syncInSqsConfig)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _integrationService = integrationService;
            _apiService = apiService;
            _syncInSqsRepository = syncOutSqsRepository;
            var syncInConfig = syncInSqsConfig.Value;
            _syncInSqsRepository.IniciarFila($"{syncInConfig.SQSBaseUrl}{syncInConfig.SQSAccessKeyId}/{syncInConfig.SQSName}");
        }

        public async Task HandleAsync(OrderCreated @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Pedido criado: {@event.HubKey}");
            if (@event != null && @event.Pedido != null)
            {
                var hubKey = @event.HubKey;
                var pedidoView = @event.Pedido;

                var integration = await _integrationService.GetIntegrationByKeyAsync(hubKey);

                var token = integration.Result!.Token ?? string.Empty;

                long terceiroId = 0;

                if (!string.IsNullOrWhiteSpace(pedidoView.ClienteCpfcnpj))
                {
                    var terceiroRequested = new Infra.ErpApi.Request.Clientes.TerceiroQueryRequest
                    {
                        Documento = pedidoView.ClienteCpfcnpj
                    };
                    var terceiroResponse = await _apiService.GetTerceirosAsync(token, terceiroRequested);
                    if (!terceiroResponse.IsSuccess)
                        return;

                    terceiroId = terceiroResponse.Result!.Any() ? terceiroResponse.Result!.FirstOrDefault()!.Id : 0;

                    if (terceiroId <= 0)
                    {
                        var terceiroRequest = BuildTerceiroRequest(pedidoView);
                        var createResponse = await _apiService.CreateTerceiroAsync(token, terceiroRequest);

                        if (createResponse?.Result == null)
                            return;

                        long.TryParse(createResponse.Result.IdRecurso, out terceiroId);
                    }
                }

                var request = VarejoOnlinePedidoMapper.Map(@event.Pedido);

                if (request != null && terceiroId > 0)
                    request.Terceiro = new TerceiroRef { Id = terceiroId };

                var operacaoResponse = await _apiService.PostPedidoAsync(token, request);

                if (pedidoView is not null && operacaoResponse.IsSuccess && operacaoResponse.Result is { } pedidoResponse)
                {
                    var retorno = BuildPedidoRetornoView(pedidoView, operacaoResponse.Result.IdRecurso);
                    var notificacao = new NotificacaoAtualizacaoModel
                    {
                        Chave = hubKey ?? string.Empty,
                        DataHora = DateTime.UtcNow,
                        Json = JsonConvert.SerializeObject(retorno),
                        TipoProcesso = TipoProcessoAtualizacao.Pedido,
                        PlataformaId = pedidoView.CanalId
                    };

                    _syncInSqsRepository.AdicionarMensagemFilaFifo(notificacao, $"notificacao-syncin-{notificacao.Chave}");
                }
            }
            return;
        }
        private static TerceiroRequest BuildTerceiroRequest(PedidoView pedidoView)
        {
            var contato = pedidoView.Contatos?.FirstOrDefault();
            var endereco = pedidoView.Enderecos?.Where(x => x.TipoEndereco.Contains("cobranca")).FirstOrDefault();

            return new TerceiroRequest
            {
                Nome = pedidoView.ClienteNome ?? string.Empty,
                Documento = pedidoView.ClienteCpfcnpj ?? string.Empty,
                Enderecos = new List<EnderecoTerceiroRequest>()
                {
                    new EnderecoTerceiroRequest
                    {
                        Tipo = "OUTROS",
                        TipoEndereco = "ENDERECO_COBRANCA",
                        Logradouro = endereco.Endereco,
                        Cep = endereco.Cep,
                        Bairro = endereco.Bairro,
                        Uf = endereco.Uf,
                        Complemento = endereco.Complemento,
                        Numero = endereco.Numero
                    }
                }
            };
        }

        private static PedidoRetornoView BuildPedidoRetornoView(PedidoView pedido, string id)
        {
            if (pedido is null) throw new ArgumentNullException(nameof(pedido));

            var statusId = pedido.PedidoStatusERPId.HasValue
                ? pedido.PedidoStatusERPId.Value > int.MaxValue
                    ? int.MaxValue
                    : (int)pedido.PedidoStatusERPId.Value
                : 0;

            return new PedidoRetornoView
            {
                PedidoId = pedido.PedidoId,
                PedidoERPId = id ?? pedido.PedidoERPId ?? string.Empty,
                PedidoERPStatusId = statusId,
                NomePlataformaERP = pedido.Plataforma ?? string.Empty,
                PedidoCancelado = pedido.PedidoCancelado,
                PedidoIncluido = true,
                PedidoAlterado = false,
                PedidoIgnorado = false,
                Erro = false,
                Mensagem = string.Empty
            };
        }
    }
}
