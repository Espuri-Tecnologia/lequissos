using System;
using System.Threading;
using System.Threading.Tasks;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Api.Controllers.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Controllers
{
    public class PedidoControllerTests
    {
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private PedidoController CreateController() => new(_dispatcher.Object);

        [Fact]
        public async Task EnviarPedido_ShouldDispatchOrderCreatedEvent_WhenPedidoIsValid()
        {
            var pedido = new PedidoView { PedidoId = 1 };
            const string hubKey = "hub";
            var controller = CreateController();

            var result = await controller.EnviarPedido(pedido, hubKey);

            Assert.IsType<OkResult>(result);
            _dispatcher.Verify(
                d => d.DispatchAsync(
                    It.Is<BaseEvent>(@event =>
                        @event is OrderCreated orderCreated &&
                        orderCreated.HubKey == hubKey &&
                        ReferenceEquals(orderCreated.Pedido, pedido)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task EnviarPedido_ShouldThrowArgumentNullException_WhenPedidoIsNull()
        {
            var controller = CreateController();

            await Assert.ThrowsAsync<ArgumentNullException>(() => controller.EnviarPedido(null!, "hub"));
            _dispatcher.Verify(
                d => d.DispatchAsync(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AlterarStatusPedidoEntregue_ShouldDispatchOrderDeliveredEvent_WhenHubKeyIsValid()
        {
            const string hubKey = "hub";
            const long pedidoErpId = 123;
            var controller = CreateController();

            var result = await controller.AlterarStatusPedidoEntregue(hubKey, pedidoErpId);

            Assert.IsType<OkResult>(result);
            _dispatcher.Verify(
                d => d.DispatchAsync(
                    It.Is<BaseEvent>(@event =>
                        @event is OrderDelivered orderDelivered &&
                        orderDelivered.HubKey == hubKey &&
                        orderDelivered.PedidoERPId == pedidoErpId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AlterarStatusPedidoEntregue_ShouldThrowArgumentNullException_WhenHubKeyIsNullOrWhitespace(string hubKey)
        {
            var controller = CreateController();

            await Assert.ThrowsAsync<ArgumentNullException>(() => controller.AlterarStatusPedidoEntregue(hubKey!, 123));
            _dispatcher.Verify(
                d => d.DispatchAsync(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task AlterarStatusPedidoEnviado_ShouldDispatchOrderShippedEvent_WhenHubKeyIsValid()
        {
            const string hubKey = "hub";
            const long pedidoErpId = 456;
            var controller = CreateController();

            var result = await controller.AlterarStatusPedidoEnviado(hubKey, pedidoErpId);

            Assert.IsType<OkResult>(result);
            _dispatcher.Verify(
                d => d.DispatchAsync(
                    It.Is<BaseEvent>(@event =>
                        @event is OrderShipped orderShipped &&
                        orderShipped.HubKey == hubKey &&
                        orderShipped.PedidoERPId == pedidoErpId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AlterarStatusPedidoEnviado_ShouldThrowArgumentNullException_WhenHubKeyIsNullOrWhitespace(string hubKey)
        {
            var controller = CreateController();

            await Assert.ThrowsAsync<ArgumentNullException>(() => controller.AlterarStatusPedidoEnviado(hubKey!, 456));
            _dispatcher.Verify(
                d => d.DispatchAsync(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
