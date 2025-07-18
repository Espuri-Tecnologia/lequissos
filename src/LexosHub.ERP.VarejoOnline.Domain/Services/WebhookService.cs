using LexosHub.ERP.VarejoOnline.Domain.DTOs.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Produto;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Webhook;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using System.Threading;
using System.Collections.Generic;
using System;

namespace LexosHub.ERP.VarejoOnline.Domain.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IWebhookRepository _webhookRepository;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejoOnlineApiService _apiService;

        public WebhookService(
            IWebhookRepository webhookRepository,
            IIntegrationService integrationService,
            IVarejoOnlineApiService apiService)
        {
            _webhookRepository = webhookRepository;
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task<Response<WebhookRecordDto>> AddAsync(WebhookRecordDto webhook)
        {
            var result = await _webhookRepository.AddAsync(webhook);
            return new Response<WebhookRecordDto>(result);
        }

        public async Task<Response<WebhookRecordDto>> RegisterAsync(WebhookDto webhookDto, CancellationToken cancellationToken = default)
        {
            if (webhookDto == null)
                throw new ArgumentNullException(nameof(webhookDto));

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(webhookDto.HubKey);

            if (integrationResponse.Result == null)
                return new Response<WebhookRecordDto> { Error = new ErrorResult("integrationNotFound") };

            var token = integrationResponse.Result.Token ?? string.Empty;

            var request = new WebhookRequest
            {
                Event = webhookDto.Event,
                url = webhookDto.Url,
                types = new List<string> { webhookDto.Method }
            };

            var result = await _apiService.RegisterWebhookAsync(token, request, cancellationToken);

            if (!result.IsSuccess || result.Result == null || string.IsNullOrWhiteSpace(result.Result.IdRecurso))
                return new Response<WebhookRecordDto> { Error = result.Error ?? new ErrorResult("registerFailed") };

            var record = new WebhookRecordDto
            {
                IntegrationId = integrationResponse.Result.Id,
                Event = webhookDto.Event,
                Method = webhookDto.Method,
                Url = webhookDto.Url,
                Uuid = result.Result.IdRecurso
            };

            var saved = await _webhookRepository.AddAsync(record);

            return new Response<WebhookRecordDto>(saved);
        }
    }
}
