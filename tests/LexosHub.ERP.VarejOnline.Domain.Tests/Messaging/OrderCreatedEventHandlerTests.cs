using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using Lexos.SQS.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class OrderCreatedEventHandlerTests
    {
        private readonly Mock<ILogger<OrderCreatedEventHandler>> _logger = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly IOptions<SyncInConfig> _syncInOptions = Options.Create(new SyncInConfig
        {
            SQSBaseUrl = "https://sqs/",
            SQSAccessKeyId = "account",
            SQSName = "queue.fifo"
        });

        private OrderCreatedEventHandler CreateHandler() =>
            new(_logger.Object, _dispatcher.Object, _integrationService.Object, _apiService.Object, _sqsRepository.Object, _syncInOptions);

        [Fact]
        public async Task HandleAsync_ShouldCreateTerceiroSendOrderAndPublishNotification()
        {
            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("hub-key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            _apiService.Setup(a => a.GetTerceirosAsync("token", It.IsAny<TerceiroQueryRequest>()))
                .ReturnsAsync(new Response<List<TerceiroResponse>>
                {
                    Result = new List<TerceiroResponse>(),
                    StatusCode = HttpStatusCode.OK
                });

            TerceiroRequest? capturedTerceiroRequest = null;
            _apiService.Setup(a => a.CreateTerceiroAsync("token", It.IsAny<TerceiroRequest>()))
                .Callback<string, TerceiroRequest>((_, request) => capturedTerceiroRequest = request)
                .ReturnsAsync(new Response<OperationResponse>
                {
                    Result = new OperationResponse { IdRecurso = "987" },
                    StatusCode = HttpStatusCode.Created
                });

            PedidoRequest? capturedPedidoRequest = null;
            _apiService.Setup(a => a.PostPedidoAsync("token", It.IsAny<PedidoRequest>()))
                .Callback<string, PedidoRequest>((_, request) => capturedPedidoRequest = request)
                .ReturnsAsync(new Response<OperationResponse>
                {
                    Result = new OperationResponse { IdRecurso = "555" },
                    StatusCode = HttpStatusCode.OK
                });

            string? startedQueueUrl = null;
            _sqsRepository.Setup(r => r.IniciarFila(It.IsAny<string>()))
                .Callback<string>(url => startedQueueUrl = url);

            NotificacaoAtualizacaoModel? publishedNotification = null;
            string? publishedGroupId = null;
            _sqsRepository.Setup(r => r.AdicionarMensagemFilaFifo(It.IsAny<NotificacaoAtualizacaoModel>(), It.IsAny<string>()))
                .Callback<NotificacaoAtualizacaoModel, string>((notification, groupId) =>
                {
                    publishedNotification = notification;
                    publishedGroupId = groupId;
                });

            var pedido = new PedidoView
            {
                PedidoId = 10,
                PedidoERPId = "existing",
                Codigo = "PED-1",
                Data = new DateTime(2024, 1, 15, 14, 30, 0, DateTimeKind.Utc),
                ClienteCpfcnpj = "12345678901",
                ClienteNome = "Cliente Teste",
                Plataforma = "Canal",
                CanalId = 7,
                LojaFaturamentoId = 99,
                PedidoStatusERPId = 12,
                Contatos = new List<PedidoClienteContatoView>
                {
                    new()
                    {
                        Nome = "Contato",
                        Telefone = "11999999999",
                        Email = "cliente@teste.com"
                    }
                },
                Enderecos = new List<PedidoClienteEnderecoView>
                {
                    new()
                    {
                        TipoEndereco = "cobranca",
                        Endereco = "Rua Cob",
                        Numero = "123",
                        Bairro = "Centro",
                        Cidade = "Cidade",
                        Uf = "SP",
                        Pais = "BR",
                        Cep = "01000-000",
                        Complemento = "Apto 1"
                    },
                    new()
                    {
                        TipoEndereco = "entrega",
                        Endereco = "Rua Ent",
                        Numero = "456",
                        Bairro = "Bairro",
                        Cidade = "Cidade",
                        Uf = "SP",
                        Pais = "BR",
                        Cep = "02000-000",
                        Complemento = "Casa"
                    }
                },
                Itens = new List<PedidoItemView>
                {
                    new()
                    {
                        Sku = "SKU1",
                        Qtde = 2,
                        Valor = 15m,
                        Desconto = 1m
                    }
                },
                ComposicaoPagamento = new List<PedidoComposicaoPagamentoView>
                {
                    new()
                    {
                        FormaPagamento = "dinheiro",
                        Valor = 29m,
                        QuantidadeParcelas = 1,
                        Data = new DateTime(2024, 1, 15)
                    }
                }
            };

            var handler = CreateHandler();

            await handler.HandleAsync(new OrderCreated
            {
                HubKey = "hub-key",
                Pedido = pedido
            }, CancellationToken.None);

            _integrationService.Verify(s => s.GetIntegrationByKeyAsync("hub-key"), Times.Once);
            _apiService.Verify(a => a.GetTerceirosAsync("token", It.Is<TerceiroQueryRequest>(r => r.Documento == "12345678901")), Times.Once);
            _apiService.Verify(a => a.CreateTerceiroAsync("token", It.IsAny<TerceiroRequest>()), Times.Once);
            _apiService.Verify(a => a.PostPedidoAsync("token", It.IsAny<PedidoRequest>()), Times.Once);
            _sqsRepository.Verify(r => r.AdicionarMensagemFilaFifo(It.IsAny<NotificacaoAtualizacaoModel>(), "notificacao-syncin-hub-key"), Times.Once);

            Assert.Equal("https://sqs/account/queue.fifo", startedQueueUrl);

            Assert.NotNull(capturedTerceiroRequest);
            Assert.Equal("Cliente Teste", capturedTerceiroRequest!.Nome);
            Assert.Equal("12345678901", capturedTerceiroRequest.Documento);
            var cobrancaEndereco = Assert.Single(capturedTerceiroRequest.Enderecos);
            Assert.Equal("Rua Cob", cobrancaEndereco.Logradouro);
            Assert.Equal("123", cobrancaEndereco.Numero);
            Assert.Equal("01000-000", cobrancaEndereco.Cep);

            Assert.NotNull(capturedPedidoRequest);
            Assert.Equal("PED-1", capturedPedidoRequest!.NumeroPedidoCliente);
            Assert.Equal("15-01-2024", capturedPedidoRequest.Data);
            Assert.Equal("14:30:00", capturedPedidoRequest.Horario);
            Assert.Equal(99, capturedPedidoRequest.Entidade!.Id);
            Assert.Equal(987, capturedPedidoRequest.Terceiro!.Id);
            var item = Assert.Single(capturedPedidoRequest.Itens!);
            Assert.Equal("SKU1", item.Produto!.CodigoSistema);
            Assert.Equal(2, item.Quantidade);
            Assert.Equal(15m, item.ValorUnitario);
            Assert.Equal(1m, item.ValorDesconto);
            Assert.Equal(29m, capturedPedidoRequest.Pagamento!.ValorDinheiro);
            Assert.Equal("Rua Ent", capturedPedidoRequest.EnderecoEntrega!.Logradouro);
            Assert.Equal("456", capturedPedidoRequest.EnderecoEntrega.Numero);

            Assert.NotNull(publishedNotification);
            Assert.Equal("hub-key", publishedNotification!.Chave);
            Assert.Equal(TipoProcessoAtualizacao.Pedido, publishedNotification.TipoProcesso);
            Assert.Equal((short)7, publishedNotification.PlataformaId);
            Assert.Equal("notificacao-syncin-hub-key", publishedGroupId);

            var retorno = JsonConvert.DeserializeObject<PedidoRetornoView>(publishedNotification.Json);
            Assert.NotNull(retorno);
            Assert.Equal(10, retorno!.PedidoId);
            Assert.Equal("555", retorno.PedidoERPId);
            Assert.Equal(12, retorno.PedidoERPStatusId);
            Assert.Equal("Canal", retorno.NomePlataformaERP);
            Assert.True(retorno.PedidoIncluido);
            Assert.False(retorno.PedidoCancelado);
            Assert.False(retorno.PedidoAlterado);
        }
    }
}

