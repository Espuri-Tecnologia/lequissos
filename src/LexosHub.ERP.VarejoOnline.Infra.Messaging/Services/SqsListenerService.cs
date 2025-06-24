using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Services
{
    public class SqsListenerService : BackgroundService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<SqsListenerService> _logger;
        private readonly IEventDispatcher _dispatcher;
        private readonly string _queueUrl;

        public SqsListenerService(IAmazonSQS sqsClient, IConfiguration configuration, ILogger<SqsListenerService> logger, IEventDispatcher dispatcher)
        {
            _sqsClient = sqsClient;
            _logger = logger;
            var baseUrl = configuration["AWS:ServiceURL"]?.TrimEnd('/');
            var queuePath = configuration["AWS:SQSQueues:IntegrationQueue"]?.TrimStart('/');
            _queueUrl = $"{baseUrl}/{queuePath}";
            _dispatcher = dispatcher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SQS Listener iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                var queueUrlResponse = await _sqsClient.GetQueueUrlAsync("integration-created-sync-varejoonline-dev");

                var request = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrlResponse.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 10
                };

                var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);

                foreach (var message in response.Messages)
                {
                    try
                    {
                        _logger.LogInformation($"Mensagem recebida: {message.Body}");

                        var eventEnvelope = JsonSerializer.Deserialize<BaseEvent>(message.Body);
                        var actualType = EventTypeResolver.Resolve(eventEnvelope.EventType);
                        var typedEvent = (BaseEvent)JsonSerializer.Deserialize(message.Body, actualType)!;

                        await _dispatcher.DispatchAsync(typedEvent, stoppingToken);

                        await _sqsClient.DeleteMessageAsync(_queueUrl, message.ReceiptHandle, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem SQS");
                    }
                }
            }
        }
    }
}
