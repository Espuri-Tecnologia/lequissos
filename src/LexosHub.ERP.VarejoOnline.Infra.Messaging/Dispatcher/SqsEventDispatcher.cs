using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher
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
            if (!_queueUrls.TryGetValue(@event.EventType, out var queueUrl))
            {
                throw new InvalidOperationException($"No queue configured for event '{@event.EventType}'");
            }

            var body = JsonSerializer.Serialize(@event, _jsonOptions);
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = body
            };

            await _sqsClient.SendMessageAsync(request, cancellationToken);
        }
    }
}
