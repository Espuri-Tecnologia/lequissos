using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Messaging
{
    public class CompaniesRequestedEventHandlerTests
    {
        private readonly Mock<ILogger<CompaniesRequestedEventHandler>> _logger = new();
        private readonly Mock<IIntegrationService> _integrationService = new();
        private readonly Mock<IVarejoOnlineApiService> _apiService = new();
        private readonly Mock<IEventDispatcher> _dispatcher = new();

        private CompaniesRequestedEventHandler CreateHandler() =>
            new CompaniesRequestedEventHandler(_logger.Object, _integrationService.Object, _apiService.Object, _dispatcher.Object);

        [Fact]
        public async Task HandleAsync_ShouldFetchIntegrationAndCallApiService()
        {
            var evt = new CompaniesRequested
            {
                HubKey = "key",
                Quantidade = 5
            };

            var integration = new IntegrationDto { Token = "token" };
            _integrationService.Setup(s => s.GetIntegrationByKeyAsync("key"))
                .ReturnsAsync(new Response<IntegrationDto>(integration));

            await CreateHandler().HandleAsync(evt, CancellationToken.None);

            _integrationService.Verify(s => s.GetIntegrationByKeyAsync("key"), Times.Once);
            _apiService.Verify(a => a.GetEmpresasAsync(
                    "token",
                    It.Is<EmpresaRequest>(r =>
                        r.Inicio == evt.Inicio &&
                        r.Quantidade == evt.Quantidade &&
                        r.AlteradoApos == evt.AlteradoApos &&
                        r.Status == evt.Status &&
                        r.CampoCustomizadoNome == evt.CampoCustomizadoNome &&
                        r.CampoCustomizadoValor == evt.CampoCustomizadoValor &&
                        r.Cnpj == evt.Cnpj
                    )
                ), Times.Once);

            _dispatcher.Verify(d => d.DispatchAsync(
                    It.Is<ProductsRequested>(p => p.HubKey == evt.HubKey),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
