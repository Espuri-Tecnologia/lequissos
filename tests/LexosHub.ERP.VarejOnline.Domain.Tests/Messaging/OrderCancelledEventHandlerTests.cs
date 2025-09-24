using System.Threading;
using System.Threading.Tasks;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class OrderCancelledEventHandlerTests
    {
        private readonly Mock<ILogger<OrderCancelledEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();

        private OrderCancelledEventHandler CreateHandler() =>
            new(_logger.Object, _integrationService.Object, _apiService.Object);

        [Fact]
        public async Task HandleAsync_ShouldCancelOrderUsingTokenAndAwaitCall()
        {
            var integration = new IntegrationDto
            {
                Token = "token"
            };

            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var tcs = new TaskCompletionSource<Response<OperationResponse>>(TaskCreationOptions.RunContinuationsAsynchronously);

            _apiService.Setup(a => a.CancelarPedidoAsync("token", 555))
                .Returns(tcs.Task);

            var handler = CreateHandler();

            var handleTask = handler.HandleAsync(new OrderCancelled
            {
                HubKey = "hub",
                PedidoERPId = 555
            }, CancellationToken.None);

            _apiService.Verify(a => a.CancelarPedidoAsync("token", 555), Times.Once);

            Assert.False(handleTask.IsCompleted);

            tcs.SetResult(new Response<OperationResponse>(new OperationResponse()));

            await handleTask;
        }
    }
}
