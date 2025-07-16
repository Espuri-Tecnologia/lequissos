using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Handlers;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Text.Json;
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
        private readonly Mock<IAmazonSQS> _sqs = new();
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"AWS:ServiceURL", "http://localhost"},
                {"AWS:SQSQueues:ProductsRequested", "queue/products"}
            })
            .Build();

        private CompaniesRequestedEventHandler CreateHandler()
        {
            var dispatcher = new SqsEventDispatcher(_sqs.Object, _configuration);
            return new CompaniesRequestedEventHandler(_logger.Object, _integrationService.Object, _apiService.Object, dispatcher);
        }

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

            _sqs.Verify(s => s.SendMessageAsync(
                It.Is<SendMessageRequest>(r => IsProductsRequestedWithHubKey(r, evt.HubKey)),
        It.IsAny<CancellationToken>()), Times.Once);

        }
        private bool IsProductsRequestedWithHubKey(SendMessageRequest request, string hubKey)
        {
            var baseEvent = JsonSerializer.Deserialize<BaseEvent>(
                request.MessageBody,
                new JsonSerializerOptions { Converters = { new BaseEventJsonConverter() } }
            );
            if (baseEvent is ProductsRequested p)
                return p.HubKey == hubKey;

            return false;
        }
    }
}
