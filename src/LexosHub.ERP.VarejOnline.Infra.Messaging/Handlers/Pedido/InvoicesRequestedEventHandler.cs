using System;
using System.Threading;
using System.Threading.Tasks;
using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class InvoicesRequestedEventHandler : IEventHandler<InvoicesRequested>
    {
        private readonly ILogger<InvoicesRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly ISqsRepository _syncOutSqsRepository;

        public InvoicesRequestedEventHandler(
            ILogger<InvoicesRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncOutConfig> syncOutConfig)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            _syncOutSqsRepository = syncOutSqsRepository;

            var config = syncOutConfig?.Value ?? throw new ArgumentNullException(nameof(syncOutConfig));
            _syncOutSqsRepository.IniciarFila($"{config.SQSBaseUrl}{config.SQSAccessKeyId}/{config.SQSName}");
        }

        public async Task HandleAsync(InvoicesRequested @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation("Invoices requested for hub {HubKey}, numero {Number}", @event.HubKey, @event.Number);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);

            if (!integrationResponse.IsSuccess || integrationResponse.Result == null)
            {
                _logger.LogWarning("Falha ao obter integração para hub {HubKey}", @event.HubKey);
                return;
            }

            var token = integrationResponse.Result.Token ?? string.Empty;

            var invoiceResponse = await _apiService.GetInvoiceXmlAsync(token, @event.Number, cancellationToken);

            var xmlContent = invoiceResponse.Result;

            if (!invoiceResponse.IsSuccess || string.IsNullOrWhiteSpace(xmlContent))
            {
                _logger.LogWarning(
                    "Não foi possível obter o XML da nota fiscal {Number} para o hub {HubKey}. Utilizando mock.",
                    @event.Number,
                    @event.HubKey);

                xmlContent = BuildMockInvoiceXml(@event.Number);
            }

            var notification = new NotificacaoAtualizacaoModel
            {
                Chave = @event.HubKey,
                DataHora = DateTime.UtcNow,
                Json = xmlContent,
                TipoProcesso = TipoProcessoAtualizacao.NotaFiscal,
                PlataformaId = 41
            };

            _syncOutSqsRepository.AdicionarMensagemFilaFifo(notification, $"notificacao-syncout-{@event.HubKey}");
        }

        private static string BuildMockInvoiceXml(long number)
        {
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                   "<NFe>" +
                   "<infNFe versao=\"4.00\" Id=\"NFe{number:D9}\">" +
                   "<ide>" +
                   "<cUF>35</cUF>" +
                   "<natOp>VENDA MOCK</natOp>" +
                   "<mod>55</mod>" +
                   "<serie>1</serie>" +
                   $"<nNF>{number}</nNF>" +
                   "<dhEmi>2025-01-01T00:00:00-03:00</dhEmi>" +
                   "</ide>" +
                   "<emit>" +
                   "<CNPJ>12345678000190</CNPJ>" +
                   "<xNome>Empresa Mock LTDA</xNome>" +
                   "</emit>" +
                   "<dest>" +
                   "<CPF>12345678909</CPF>" +
                   "<xNome>Cliente Mock</xNome>" +
                   "</dest>" +
                   "</infNFe>" +
                   "</NFe>";
        }
    }
}
