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
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
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
    public class OrderDeliveredEventHandlerTests
    {
        private readonly Mock<ILogger<OrderDeliveredEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly IOptions<SyncInConfig> _syncInOptions = Options.Create(new SyncInConfig
        {
            SQSBaseUrl = "https://sqs/",
            SQSAccessKeyId = "account",
            SQSName = "queue.fifo"
        });

        private OrderDeliveredEventHandler CreateHandler() =>
            new(_logger.Object, _integrationService.Object, _apiService.Object, _sqsRepository.Object, _syncInOptions);

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

            var handleTask = handler.HandleAsync(new OrderDelivered
            {
                HubKey = "hub",
                PedidoERPId = 987,
                Pedido = new PedidoView
                {
                    PedidoId = 456,
                    CanalId = 9,
                    Plataforma = "Marketplace"
                }
            }, CancellationToken.None);

            _apiService.Verify(a => a.AlterarStatusPedidoAsync("token", It.Is<AlterarStatusPedidoRequest>(r =>
                r.IdPedido == 987 && r.StatusPedidoVenda!.Id == 321)), Times.Once);

            Assert.False(handleTask.IsCompleted);

            tcs.SetResult(new Response<OperationResponse>(new OperationResponse { IdRecurso = "555" }));

            await handleTask;

            Assert.Equal("https://sqs/account/queue.fifo", startedQueue);
            Assert.NotNull(publishedNotification);
            Assert.Equal("hub", publishedNotification!.Chave);
            Assert.Equal(TipoProcessoAtualizacao.Pedido, publishedNotification.TipoProcesso);
            Assert.Equal((short)9, publishedNotification.PlataformaId);
            Assert.Equal("notificacao-syncin-hub", publishedGroupId);

            var retorno = JsonConvert.DeserializeObject<PedidoRetornoView>(publishedNotification.Json);
            Assert.NotNull(retorno);
            Assert.Equal(456, retorno!.PedidoId);
            Assert.Equal("555", retorno.PedidoERPId);
            Assert.Equal(321, retorno.PedidoERPStatusId);
            Assert.False(retorno.PedidoIncluido);
            Assert.True(retorno.PedidoAlterado);
            Assert.False(retorno.PedidoCancelado);
        }
    }
}

