using System;
using System.Threading;
using System.Threading.Tasks;
using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.Hub.Sync.Models.Pedido;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class OrderCancelledEventHandlerTests
    {
        private readonly Mock<ILogger<OrderCancelledEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly IOptions<SyncInConfig> _syncInOptions = Options.Create(new SyncInConfig
        {
            SQSBaseUrl = "https://sqs/",
            SQSAccessKeyId = "account",
            SQSName = "queue.fifo"
        });

        private OrderCancelledEventHandler CreateHandler() =>
            new(_logger.Object, _integrationService.Object, _apiService.Object, _sqsRepository.Object, _syncInOptions);

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

            string? startedQueue = null;
            _sqsRepository.Setup(r => r.IniciarFila(It.IsAny<string>()))
                .Callback<string>(url => startedQueue = url);

            NotificacaoAtualizacaoModel? publishedNotification = null;
            string? publishedGroupId = null;
            _sqsRepository.Setup(r => r.AdicionarMensagemFilaFifo(It.IsAny<NotificacaoAtualizacaoModel>(), It.IsAny<string>()))
                .Callback<NotificacaoAtualizacaoModel, string>((notification, groupId) =>
                {
                    publishedNotification = notification;
                    publishedGroupId = groupId;
                });

            var handler = CreateHandler();

            var handleTask = handler.HandleAsync(new OrderCancelled
            {
                HubKey = "hub",
                PedidoERPId = 555,
                Pedido = new PedidoView
                {
                    PedidoId = 123,
                    CanalId = 7,
                    Plataforma = "Canal",
                    PedidoCancelado = false
                }
            }, CancellationToken.None);

            _apiService.Verify(a => a.CancelarPedidoAsync("token", 555), Times.Once);

            Assert.False(handleTask.IsCompleted);

            tcs.SetResult(new Response<OperationResponse>(new OperationResponse { IdRecurso = "999" }));

            await handleTask;

            Assert.Equal("https://sqs/account/queue.fifo", startedQueue);
            Assert.NotNull(publishedNotification);
            Assert.Equal("hub", publishedNotification!.Chave);
            Assert.Equal(TipoProcessoAtualizacao.Pedido, publishedNotification.TipoProcesso);
            Assert.Equal((short)7, publishedNotification.PlataformaId);
            Assert.Equal("notificacao-syncin-hub", publishedGroupId);

            var retorno = JsonConvert.DeserializeObject<PedidoRetornoView>(publishedNotification.Json);
            Assert.NotNull(retorno);
            Assert.Equal(123, retorno!.PedidoId);
            Assert.Equal("999", retorno.PedidoERPId);
            Assert.True(retorno.PedidoCancelado);
            Assert.False(retorno.PedidoIncluido);
            Assert.False(retorno.PedidoAlterado);
        }
    }
}
