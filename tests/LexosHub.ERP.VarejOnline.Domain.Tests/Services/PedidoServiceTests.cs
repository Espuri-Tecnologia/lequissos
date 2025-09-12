using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Services;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses.Clientes;
using Moq;
using Xunit;
using System;

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
        public async Task EnviarPedido_ShouldCreateTerceiroAndSendMappedPedido_WhenClienteNotFound()
        {
            var pedidoView = new PedidoView
            {
                Codigo = "PED-1",
                Data = new DateTime(2024, 1, 15, 14, 30, 0),
                ClienteCpfcnpj = "123",
                ClienteNome = "Teste",
                Contatos = new List<PedidoClienteContatoView>
                {
                    new() { Nome = "Contato", Telefone = "999", Email = "c@t.com" }
                },
                Enderecos = new List<PedidoClienteEnderecoView>
                {
                    new()
                    {
                        TipoEndereco = "entrega",
                        Endereco = "Rua A",
                        Numero = "10",
                        Bairro = "Centro",
                        Cidade = "Cidade",
                        Uf = "SP",
                        Pais = "BR",
                        Cep = "00000-000",
                        Complemento = "Ap"
                    }
                },
                Itens = new List<PedidoItemView>
                {
                    new() { Sku = "SKU1", Qtde = 2, Valor = 10m, Desconto = 1m }
                },
                ComposicaoPagamento = new List<PedidoComposicaoPagamentoView>
                {
                    new() { FormaPagamento = "dinheiro", Valor = 20m, QuantidadeParcelas = 1, Data = new DateTime(2024,1,15) }
                }
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub"))
                .ReturnsAsync(new Response<IntegrationDto> { Result = integration });

            _apiService.Setup(a => a.GetTerceirosAsync("token", It.IsAny<TerceiroQueryRequest>()))
                .ReturnsAsync(new Response<List<TerceiroResponse>> { Result = new List<TerceiroResponse>(), StatusCode = HttpStatusCode.OK });

            _apiService.Setup(a => a.CreateTerceiroAsync("token", It.IsAny<TerceiroRequest>()))
                .ReturnsAsync(new Response<TerceiroResponse> { Result = new TerceiroResponse { Id = 123 }, StatusCode = HttpStatusCode.OK });

            PedidoRequest? capturedRequest = null;
            _apiService.Setup(a => a.PostPedidoAsync("token", It.IsAny<PedidoRequest>()))
                .Callback((string _, PedidoRequest req) => capturedRequest = req)
                .ReturnsAsync(new Response<PedidoResponse> { Result = new PedidoResponse(), StatusCode = HttpStatusCode.OK });

            var service = CreateService();
            var result = await service.EnviarPedido("hub", pedidoView);

            Assert.True(result.IsSuccess);
            _apiService.Verify(a => a.GetTerceirosAsync("token", It.Is<TerceiroQueryRequest>(r => r.Documento == "123")), Times.Once);
            _apiService.Verify(a => a.CreateTerceiroAsync("token", It.IsAny<TerceiroRequest>()), Times.Once);
            _apiService.Verify(a => a.PostPedidoAsync("token", It.IsAny<PedidoRequest>()), Times.Once);

            Assert.NotNull(capturedRequest);
            Assert.Equal("PED-1", capturedRequest!.NumeroPedidoCliente);
            Assert.Equal("15-01-2024", capturedRequest.Data);
            Assert.Equal("14:30:00", capturedRequest.Horario);
            Assert.NotNull(capturedRequest.Terceiro);
            Assert.Equal(123, capturedRequest.Terceiro!.Id);
            var item = Assert.Single(capturedRequest.Itens!);
            Assert.Equal("SKU1", item.Produto.CodigoSistema);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(10m, item.ValorUnitario);
            Assert.Equal(1m, item.ValorDesconto);
            Assert.NotNull(capturedRequest.Pagamento);
            Assert.Equal(20m, capturedRequest.Pagamento!.ValorDinheiro);
            Assert.NotNull(capturedRequest.EnderecoEntrega);
            Assert.Equal("Rua A", capturedRequest.EnderecoEntrega!.Logradouro);
            Assert.Equal("10", capturedRequest.EnderecoEntrega.Numero);
            Assert.Equal("Centro", capturedRequest.EnderecoEntrega.Bairro);
            Assert.Equal("Cidade", capturedRequest.EnderecoEntrega.Cidade);
            Assert.Equal("SP", capturedRequest.EnderecoEntrega.Uf);
        }
    }
}
