using LexosHub.ERP.VarejOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Webhook;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Webhook
{
    public class WebhookRepository : IWebhookRepository
    {
        private readonly IApplicationWriteDbConnection _writeDbConnection;
        private readonly ILogger<WebhookRepository> _logger;

        public WebhookRepository(IApplicationWriteDbConnection writeDbConnection, ILogger<WebhookRepository> logger)
        {
            _writeDbConnection = writeDbConnection;
            _logger = logger;
        }

        public async Task<WebhookRecordDto> AddAsync(WebhookRecordDto webhook)
        {
            try
            {
                var id = await _writeDbConnection.ExecuteScalarAsync<int>(
                    sql: @"INSERT INTO [Webhook] ([IntegrationId], [Uuid], [Event], [Types], [Url], [CreatedDate], [UpdatedDate])
                           OUTPUT INSERTED.Id
                           VALUES (@IntegrationId, @Uuid, @Event, @Types, @Url, GETDATE(), GETDATE());",
                    param: new
                    {
                        webhook.IntegrationId,
                        webhook.Uuid,
                        webhook.Event,
                        Types = string.Join(',', webhook.Types ?? new List<string>()),
                        webhook.Url
                    });
                webhook.Id = id;
                return webhook;
            }
            catch (Exception e)
            {
                _logger.LogError("----- WebhookRepository -> Error on AddAsync {error} ----", e.Message);
                throw;
            }
        }
    }
}
