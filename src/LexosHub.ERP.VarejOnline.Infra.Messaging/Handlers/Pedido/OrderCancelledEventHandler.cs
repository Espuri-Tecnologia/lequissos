using System;
using System.Threading;
using System.Threading.Tasks;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public class OrderCancelledEventHandler : PedidoEventHandlerBase, IEventHandler<OrderCancelled>
    {
        private readonly ILogger<OrderCancelledEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderCancelledEventHandler(
            ILogger<OrderCancelledEventHandler> logger,
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

        public async Task HandleAsync(OrderCancelled @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation("Cancelamento de Pedido | PedidoERPId:  {PedidoERPId}", @event.PedidoERPId);

            if (@event.Pedido == null)
            {
                _logger.LogWarning("Pedido não informado para cancelamento do pedido ERP {PedidoERPId}", @event.PedidoERPId);
                return;
            }

            var integration = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            var token = integration.Result?.Token ?? string.Empty;

            var response = await _apiService.CancelarPedidoAsync(token, @event.PedidoERPId);

            if (!response.IsSuccess)
            {
                _logger.LogError("Falha ao cancelar o pedido {PedidoERPId}: {Erro}", @event.PedidoERPId, response.Error?.Message);
                return;
            }

            var pedidoView = @event.Pedido;
            pedidoView.PedidoCancelado = true;
            pedidoView.PedidoERPId ??= @event.PedidoERPId.ToString();

            var recursoId = response.Result?.IdRecurso ?? @event.PedidoERPId.ToString();

            var retorno = BuildPedidoRetornoView(pedidoView, recursoId, pedidoCancelado: true);
            PublishPedidoRetorno(@event.HubKey, pedidoView.CanalId, retorno);
        }
    }
}
