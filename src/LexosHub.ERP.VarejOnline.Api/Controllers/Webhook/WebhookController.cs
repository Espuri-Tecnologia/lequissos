using LexosHub.ERP.VarejOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/webhook")]
[Produces("application/json")]
public class WebhookController : ControllerBase
{
    private readonly IWebhookService _webhookService;
    private readonly IEventDispatcher _dispatcher;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IEventDispatcher dispatcher,
        ILogger<WebhookController> logger,
        IWebhookService webhookService)
    {
        _dispatcher = dispatcher;
        _logger = logger;
        _webhookService = webhookService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterGenericWebhook([FromBody] WebhookDto webhookDto)
    {
        if (webhookDto == null)
            return BadRequest(new { error = "Payload vazio" });

        await _webhookService.RegisterAsync(webhookDto);
        return Ok(new { message = "Webhook recebido com sucesso." });
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
            return BadRequest(new { error = "ID do produto inv�lido na notifica��o" });

        var evt = new ProductsRequested
        {
            HubKey = hubkey,
            Id = productId
        };

        await _dispatcher.DispatchAsync(evt, cancellationToken);

        _logger.LogInformation("Evento ProductsRequested disparado para productId={ProductId}", productId);

        return Ok(new { message = "Notifica��o de produto processada com sucesso." });
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
            return BadRequest(new { error = "ID da tabela de pre�o inv�lido na notifica��o" });

        var evt = new PriceTablesRequested
        {
            HubKey = hubkey,
            Id = tabelaPrecoId.Value
        };

        await _dispatcher.DispatchAsync(evt, cancellationToken);

        _logger.LogInformation("Evento PriceTablesRequested disparado para tabelaPrecoId={TabelaPrecoId}", tabelaPrecoId);

        return Ok(new { message = "Notifica��o de tabela de pre�o processada com sucesso." });
    }
    private long? ExtractObjectId(string obj)
    {
        if (string.IsNullOrWhiteSpace(obj))
            return null;

        var lastSegment = obj.TrimEnd('/').Split('/').LastOrDefault();
        return long.TryParse(lastSegment, out var id) ? id : null;
    }
}
