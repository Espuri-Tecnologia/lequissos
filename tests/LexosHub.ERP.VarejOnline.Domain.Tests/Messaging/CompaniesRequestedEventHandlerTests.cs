using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Messaging
{
    public class CompaniesRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<CompaniesRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejOnlineApiService> _apiService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private CompaniesRequestedEventHandler CreateHandler()
        {
            _dispatcher
                .Setup(d => d.DispatchAsync(It.IsAny<BaseEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return new CompaniesRequestedEventHandler(
                _logger.Object,
                _integrationService.Object,
                _apiService.Object,
                _dispatcher.Object);
        }

        [Fact]
        public async Task HandleAsync_ShouldFetchIntegration_CallApiService_AndDispatchProductsRequested()
        {
            var evt = new CompaniesRequested
            {
                HubKey = "key",
                Inicio = 1,
                Quantidade = 5,
                AlteradoApos = "2024-01-01",
                Status = "ATIVO",
                CampoCustomizadoNome = "Canal",
                CampoCustomizadoValor = "Online",
                Cnpj = "12345678000190"
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService
                .Setup(s => s.GetIntegrationByKeyAsync(evt.HubKey))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            var handler = CreateHandler();

            await handler.HandleAsync(evt, CancellationToken.None);

            _integrationService.Verify(s => s.GetIntegrationByKeyAsync(evt.HubKey), Times.Once);

            _apiService.Verify(a => a.GetEmpresasAsync(
                    integration.Token,
                    It.Is<EmpresaRequest>(r =>
                        r.Inicio == evt.Inicio &&
                        r.Quantidade == evt.Quantidade &&
                        r.AlteradoApos == evt.AlteradoApos &&
                        r.Status == evt.Status &&
                        r.CampoCustomizadoNome == evt.CampoCustomizadoNome &&
                        r.CampoCustomizadoValor == evt.CampoCustomizadoValor &&
                        r.Cnpj == evt.Cnpj)),
                Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<BaseEvent>(e => e is ProductsRequested p && p.HubKey == evt.HubKey),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
