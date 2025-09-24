using System;
using System.Threading;
using System.Threading.Tasks;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public class OrderShippedEventHandler : PedidoEventHandlerBase, IEventHandler<OrderShipped>
    {
        private readonly ILogger<OrderShippedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderShippedEventHandler(
            ILogger<OrderShippedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            ISqsRepository syncInSqsRepository,
            IOptions<SyncInConfig> syncInSqsConfig)
            : base(syncInSqsRepository, syncInSqsConfig)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task HandleAsync(OrderShipped @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation("Alteração de Status (Enviado) | PedidoERPId:  {PedidoERPId}", @event.PedidoERPId);

            if (@event.Pedido == null)
            {
                _logger.LogWarning("Pedido não informado para atualização de status enviado do pedido ERP {PedidoERPId}", @event.PedidoERPId);
                return;
            }

            var integration = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integration.Result?.Token ?? string.Empty;
            var statusShipped = integration.Result?.Settings?.StatusShipped;

            var request = new AlterarStatusPedidoRequest
            {
                IdPedido = @event.PedidoERPId,
                StatusPedidoVenda = new StatusPedidoVendaRequest
                {
                    Id = statusShipped
                }
            };

            var response = await _apiService.AlterarStatusPedidoAsync(token, request);

            if (!response.IsSuccess)
            {
                _logger.LogError("Falha ao alterar status para enviado do pedido {PedidoERPId}: {Erro}", @event.PedidoERPId, response.Error?.Message);
                return;
            }

            var pedidoView = @event.Pedido;
            pedidoView.PedidoERPId ??= @event.PedidoERPId.ToString();
            pedidoView.PedidoStatusERPId = statusShipped;

            var recursoId = response.Result?.IdRecurso ?? @event.PedidoERPId.ToString();
            var retorno = BuildPedidoRetornoView(pedidoView, recursoId, pedidoAlterado: true);
            PublishPedidoRetorno(@event.HubKey, pedidoView.CanalId, retorno);
        }
    }
}
