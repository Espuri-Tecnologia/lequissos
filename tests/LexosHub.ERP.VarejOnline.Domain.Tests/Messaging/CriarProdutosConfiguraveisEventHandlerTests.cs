using Lexos.Hub.Sync;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Lexos.SQS.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class CriarProdutosConfiguraveisEventHandlerTests
    {
        private readonly Mock<ILogger<CriarProdutosConfiguraveisEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<ISqsRepository> _sqsRepository = new();
        private readonly IOptions<SyncOutConfig> _options = Options.Create(new SyncOutConfig());

        private CriarProdutosConfiguraveisEventHandler CreateHandler() =>
            new(_logger.Object, _integrationService.Object, _apiService.Object, _sqsRepository.Object, _options);

        [Fact]
        public async Task HandleAsync_ShouldCallApiAndSendMessage()
        {
            var evt = new CriarProdutosConfiguraveis
            {
                HubKey = "key",
                Produtos = new List<ProdutoResponse>
                {
                    new ProdutoResponse
                    {
                        Id = 1,
                        Descricao = "Produto",
                        CodigoSistema = "SKU",
                        CodigoSku = "SKU",
                        MercadoriaBase = true,
                        Ativo = true
                    }
                }
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var variacoes = new List<ProdutoResponse>
            {
                new ProdutoResponse
                {
                    Id = 2,
                    Descricao = "Var",
                    CodigoSistema = "V1",
                    CodigoSku = "V1",
                    Ativo = true
                }
            };

            _apiService.Setup(a => a.GetProdutosAsync("token", It.Is<ProdutoRequest>(r => r.ProdutoBase == 1)))
                .ReturnsAsync(new Response<List<ProdutoResponse>>(variacoes));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _apiService.Verify(a => a.GetProdutosAsync("token", It.Is<ProdutoRequest>(r => r.ProdutoBase == 1)), Times.Once);
            _sqsRepository.Verify(r => r.AdicionarMensagemFilaFifo(
                It.IsAny<NotificacaoAtualizacaoModel>(),
                It.Is<string>(s => s == "notificacao-syncout-key")), Times.Once);
        }
    }
}

