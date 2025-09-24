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
    public class OrderDeliveredEventHandler : PedidoEventHandlerBase, IEventHandler<OrderDelivered>
    {
        private readonly ILogger<OrderDeliveredEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderDeliveredEventHandler(
            ILogger<OrderDeliveredEventHandler> logger,
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

        public async Task HandleAsync(OrderDelivered @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation("Alteração de Status (Entregue) | PedidoERPId:  {PedidoERPId}", @event.PedidoERPId);

            if (@event.Pedido == null)
            {
                _logger.LogWarning("Pedido não informado para atualização de status entregue do pedido ERP {PedidoERPId}", @event.PedidoERPId);
                return;
            }

            var integration = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integration.Result?.Token ?? string.Empty;
            var statusDelivered = integration.Result?.Settings?.StatusDelivered;

            var request = new AlterarStatusPedidoRequest
            {
                IdPedido = @event.PedidoERPId,
                StatusPedidoVenda = new StatusPedidoVendaRequest
                {
                    Id = statusDelivered
                }
            };

            var response = await _apiService.AlterarStatusPedidoAsync(token, request);

            if (!response.IsSuccess)
            {
                _logger.LogError("Falha ao alterar status para entregue do pedido {PedidoERPId}: {Erro}", @event.PedidoERPId, response.Error?.Message);
                return;
            }

            var pedidoView = @event.Pedido;
            pedidoView.PedidoERPId ??= @event.PedidoERPId.ToString();
            pedidoView.PedidoStatusERPId = statusDelivered;

            var recursoId = response.Result?.IdRecurso ?? @event.PedidoERPId.ToString();
            var retorno = BuildPedidoRetornoView(pedidoView, recursoId, pedidoAlterado: true);
            PublishPedidoRetorno(@event.HubKey, pedidoView.CanalId, retorno);
        }
    }
}
