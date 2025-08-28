using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Moq;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Services
{
    public class PedidoServiceTests
    {
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();

        private PedidoService CreateService() => new(_integrationService.Object, _apiService.Object);

        [Fact]
        public async Task AlterarStatusPedido_ShouldCallApiWithToken()
        {
            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto> { Result = integration });

            var apiResponse = new Response<PedidoResponse> { Result = new PedidoResponse(), StatusCode = HttpStatusCode.OK };
            _apiService.Setup(a => a.AlterarStatusPedidoAsync("token", 10, "novo"))
                .ReturnsAsync(apiResponse);

            var service = CreateService();
            var result = await service.AlterarStatusPedido("hub", 10, "novo");

            Assert.Same(apiResponse, result);
            _apiService.Verify(a => a.AlterarStatusPedidoAsync("token", 10, "novo"), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusPedido_ShouldReturnError_WhenIntegrationNotFound()
        {
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto> { Error = new ErrorResult("notfound") });

            var service = CreateService();
            var result = await service.AlterarStatusPedido("hub", 10, "novo");

            Assert.False(result.IsSuccess);
            _apiService.Verify(a => a.AlterarStatusPedidoAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EnviarPedido_ShouldCreateTerceiro_WhenNotFound()
        {
            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto> { Result = integration });

            var pedidoView = new PedidoView
            {
                ClienteCpfcnpj = "123",
                ClienteNome = "Cliente",
                Contatos = new List<PedidoClienteContatoView> { new() { Nome = "Contato", Telefone = "999", Email = "c@e.com" } },
                Enderecos = new List<PedidoClienteEnderecoView> { new() { Endereco = "Rua", Numero = "1", Bairro = "Bairro", Cidade = "Cidade", Uf = "UF", Pais = "BR", Cep = "123" } }
            };

            _apiService.Setup(a => a.GetTerceiroByDocumentoAsync("token", "123"))
                .ReturnsAsync(new Response<List<TerceiroResponse>> { Result = new List<TerceiroResponse>() });

            _apiService.Setup(a => a.CreateTerceiroAsync("token", It.Is<TerceiroRequest>(t => t.Documento == "123")))
                .ReturnsAsync(new Response<TerceiroResponse?> { Result = new TerceiroResponse { Id = 10 } });

            _apiService.Setup(a => a.PostPedidoAsync("token", It.Is<PedidoRequest>(p => p.Terceiro?.Id == 10)))
                .ReturnsAsync(new Response<PedidoResponse> { Result = new PedidoResponse() });

            var service = CreateService();
            var result = await service.EnviarPedido("hub", pedidoView);

            Assert.True(result.IsSuccess);
            _apiService.Verify(a => a.CreateTerceiroAsync("token", It.IsAny<TerceiroRequest>()), Times.Once);
        }

        [Fact]
        public async Task EnviarPedido_ShouldAbort_WhenTerceiroCreationFails()
        {
            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto> { Result = integration });

            var pedidoView = new PedidoView { ClienteCpfcnpj = "123", ClienteNome = "Cliente" };

            _apiService.Setup(a => a.GetTerceiroByDocumentoAsync("token", "123"))
                .ReturnsAsync(new Response<List<TerceiroResponse>> { Result = new List<TerceiroResponse>() });

            _apiService.Setup(a => a.CreateTerceiroAsync("token", It.IsAny<TerceiroRequest>()))
                .ReturnsAsync(new Response<TerceiroResponse?> { Error = new ErrorResult("fail") });

            var service = CreateService();
            var result = await service.EnviarPedido("hub", pedidoView);

            Assert.False(result.IsSuccess);
            _apiService.Verify(a => a.PostPedidoAsync(It.IsAny<string>(), It.IsAny<PedidoRequest>()), Times.Never);
        }
    }
}
