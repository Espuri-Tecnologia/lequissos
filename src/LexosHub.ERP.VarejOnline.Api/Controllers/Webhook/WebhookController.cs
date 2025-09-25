using LexosHub.ERP.VarejOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

[ApiController]
[Route("api/webhook")]
[Produces("application/json")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookService _webhookService;
    private readonly ISqslEventPublisher _publisher;
    private readonly IEventDispatcher _dispatcher;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        ISqslEventPublisher publisher,
        ILogger<WebhookController> logger,
        IWebhookService webhookService,
        IEventDispatcher dispatcher)
    {
        _publisher = publisher;
        _logger = logger;
        _webhookService = webhookService;
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterGenericWebhook([FromBody] WebhookDto webhookDto)
    {
        if (webhookDto == null)
            return BadRequest(new { error = "Payload vazio" });

        await _webhookService.RegisterAsync(webhookDto);
        return Ok(new { message = "Webhook recebido com sucesso." });
    }

    [HttpPost("{hubkey}")]
    public async Task<IActionResult> PublishProductsRequested([FromRoute] string hubkey, CancellationToken cancellationToken)
    {

        var evt = new ProductsRequested
        {
            HubKey = hubkey
        };

        await _publisher.DispatchAsync(evt, cancellationToken);

        return Ok(new { message = "Notificação de produto processada com sucesso." });
    }

    [HttpPost("{hubkey}/produto")]
    public async Task<IActionResult> PostProdutoWebhookAsync(
        [FromBody] WebhookNotificationDto notification,
        [FromRoute] string hubkey,
        CancellationToken cancellationToken)
    {
        if (notification == null)
            return BadRequest(new { error = "Payload vazio" });

        using var scope = _logger.BeginScope("Webhook Produto | Hub: {hubkey}", hubkey);
        _logger.LogInformation("Recebido payload: {@Payload}", notification);

        long? productId = ExtractObjectId(notification.Object);

        if (productId == null)
            return BadRequest(new { error = "ID do produto inválido na notificação" });

        var evt = new ProductsRequested
        {
            HubKey = hubkey,
            Id = productId
        };

        await _publisher.DispatchAsync(evt, cancellationToken);

        _logger.LogInformation("Evento ProductsRequested disparado para productId={ProductId}", productId);

        return Ok(new { message = "Notificação de produto processada com sucesso." });
    }

    [HttpPost("{hubkey}/tabela-preco")]
    public async Task<IActionResult> PostTabelaPrecoWebhookAsync(
        [FromBody] WebhookNotificationDto notification,
        [FromRoute] string hubkey,
        CancellationToken cancellationToken)
    {
        if (notification == null)
            return BadRequest(new { error = "Payload vazio" });

        using var scope = _logger.BeginScope("Webhook TabelaPreco | Hub: {hubkey}", hubkey);
        _logger.LogInformation("Recebido payload: {@Payload}", notification);

        int? tabelaPrecoId = (int?)ExtractObjectId(notification.Object);

        if (tabelaPrecoId == null)
            return BadRequest(new { error = "ID da tabela de preço inválido na notificação" });

        var evt = new PriceTablesRequested
        {
            HubKey = hubkey,
            Id = tabelaPrecoId.Value
        };

        await _publisher.DispatchAsync(evt, cancellationToken);

        _logger.LogInformation("Evento PriceTablesRequested disparado para tabelaPrecoId={TabelaPrecoId}", tabelaPrecoId);

        return Ok(new { message = "Notificação de tabela de preço processada com sucesso." });
    }

    [HttpPost("{hubkey}/nota-fiscal")]
    public async Task<IActionResult> PostNotaFiscalWebhookAsync(
        [FromBody] WebhookNotificationDto notification,
        [FromRoute] string hubkey,
        CancellationToken cancellationToken)
    {
        if (notification == null)
            return BadRequest(new { error = "Payload vazio" });

        using var scope = _logger.BeginScope("Webhook NotaFiscal | Hub: {hubkey}", hubkey);
        _logger.LogInformation("Recebido payload: {@Payload}", notification);

        long? erpNotaFiscalId = ExtractObjectId(notification.Object);

        if (erpNotaFiscalId == null)
            return BadRequest(new { error = "Id da nota fiscal não enviado na Notificação" });

        var evt = new InvoicesRequested
        {
            HubKey = hubkey,
            Number = erpNotaFiscalId.Value
        };

        await _dispatcher.DispatchAsync(evt, cancellationToken);

        return Ok(new { message = "Notificação de nota fiscal processada com sucesso." });
    }
    private long? ExtractObjectId(string obj)
    {
        if (string.IsNullOrWhiteSpace(obj))
            return null;

        var lastSegment = obj.TrimEnd('/').Split('/').LastOrDefault();
        return long.TryParse(lastSegment, out var id) ? id : null;
    }
}
