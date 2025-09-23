//using System.Net;
//using System.Threading.Tasks;
//using LexosHub.ERP.VarejOnline.Api.Controllers.Pedido;
//using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
//using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
//using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using Xunit;

//namespace LexosHub.ERP.VarejOnline.Domain.Tests.Controllers
//{
//    public class PedidoControllerTests
//    {
//        private readonly Mock<IPedidoService> _service = new();
//        private PedidoController CreateController() => new(_service.Object);

//        [Fact]
//        public async Task AlterarStatusPedido_ShouldReturnOk_WhenSuccess()
//        {
//            var response = new Response<PedidoResponse> { Result = new PedidoResponse(), StatusCode = HttpStatusCode.OK };
//            _service.Setup(s => s.AlterarStatusPedido("hub", 1, "novo")).ReturnsAsync(response);

//            var controller = CreateController();
//            var result = await controller.AlterarStatusPedido("hub", 1, "novo");

//            var ok = Assert.IsType<OkObjectResult>(result);
//            Assert.Same(response, ok.Value);
//            _service.Verify(s => s.AlterarStatusPedido("hub", 1, "novo"), Times.Once);
//        }

//        [Fact]
//        public async Task AlterarStatusPedido_ShouldReturnBadRequest_WhenServiceReturnsBadRequest()
//        {
//            var response = new Response<PedidoResponse> { Error = new ErrorResult("bad"), StatusCode = HttpStatusCode.BadRequest };
//            _service.Setup(s => s.AlterarStatusPedido("hub", 1, "novo")).ReturnsAsync(response);

//            var controller = CreateController();
//            var result = await controller.AlterarStatusPedido("hub", 1, "novo");

//            var bad = Assert.IsType<BadRequestObjectResult>(result);
//            Assert.Same(response, bad.Value);
//        }

//        [Fact]
//        public async Task AlterarStatusPedido_ShouldReturnConflict_WhenServiceReturnsConflict()
//        {
//            var response = new Response<PedidoResponse> { Error = new ErrorResult("conflict"), StatusCode = HttpStatusCode.Conflict };
//            _service.Setup(s => s.AlterarStatusPedido("hub", 1, "novo")).ReturnsAsync(response);

//            var controller = CreateController();
//            var result = await controller.AlterarStatusPedido("hub", 1, "novo");

//            var conflict = Assert.IsType<ConflictObjectResult>(result);
//            Assert.Same(response, conflict.Value);
//        }
//    }
//}
