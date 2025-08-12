using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public class SqsEventDispatcher : IEventDispatcher
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly Dictionary<string, string> _queueUrls;
        private readonly JsonSerializerOptions _jsonOptions;

        public SqsEventDispatcher(IAmazonSQS sqsClient, IConfiguration configuration)
        {
            _sqsClient = sqsClient;

            var baseUrl = configuration["AWS:ServiceURL"]?.TrimEnd('/');
            _queueUrls = configuration.GetSection("AWS:SQSQueues").Get<Dictionary<string, string>>() ?? new();

            if (!string.IsNullOrEmpty(baseUrl))
            {
                foreach (var key in _queueUrls.Keys.ToList())
                {
                    var value = _queueUrls[key];
                    _queueUrls[key] = value.StartsWith("http") ? value : $"{baseUrl}/{value.TrimStart('/')}";
                }
            }

            _jsonOptions = new JsonSerializerOptions();
            _jsonOptions.Converters.Add(new BaseEventJsonConverter());
        }

        public async Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken)
        {
            var key = @event.EventType switch
            {
                nameof(CriarProdutosSimples) or nameof(CriarProdutosConfiguraveis) or nameof(CriarProdutosKits) => "Produtos",
                _ => @event.EventType
            };

            if (!_queueUrls.TryGetValue(key, out var queueUrl))
            {
                throw new InvalidOperationException($"No queue configured for event '{@event.EventType}'");
            }

            var body = JsonSerializer.Serialize(@event, _jsonOptions);
            var groupId = @event.GetType().GetProperty("HubKey")?.GetValue(@event)?.ToString() ?? @event.EventType;
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = body,
                MessageGroupId = groupId,
                MessageDeduplicationId = Guid.NewGuid().ToString()
            };

            await _sqsClient.SendMessageAsync(request, cancellationToken);
        }
    }
}
