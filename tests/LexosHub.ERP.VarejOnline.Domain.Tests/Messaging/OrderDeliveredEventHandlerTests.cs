using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class OrderDeliveredEventHandlerTests
    {
        private readonly Mock<ILogger<OrderDeliveredEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();

        private OrderDeliveredEventHandler CreateHandler() =>
            new(_logger.Object, _integrationService.Object, _apiService.Object);

        [Fact]
        public async Task HandleAsync_ShouldUpdateStatusUsingDeliveredSettingAndAwaitCall()
        {
            var integration = new IntegrationDto
            {
                Token = "token",
                Settings = new Settings
                {
                    StatusDelivered = 321
                }
            };

            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var tcs = new TaskCompletionSource<Response<OperationResponse>>(TaskCreationOptions.RunContinuationsAsynchronously);

            _apiService.Setup(a => a.AlterarStatusPedidoAsync("token", It.IsAny<AlterarStatusPedidoRequest>()))
                .Returns(tcs.Task);

            var handler = CreateHandler();

            var handleTask = handler.HandleAsync(new OrderDelivered
            {
                HubKey = "hub",
                PedidoERPId = 987
            }, CancellationToken.None);

            _apiService.Verify(a => a.AlterarStatusPedidoAsync("token", It.Is<AlterarStatusPedidoRequest>(r =>
                r.IdPedido == 987 && r.StatusPedidoVenda!.Id == 321)), Times.Once);

            Assert.False(handleTask.IsCompleted);

            tcs.SetResult(new Response<OperationResponse>(new OperationResponse()));

            await handleTask;
        }
    }
}

