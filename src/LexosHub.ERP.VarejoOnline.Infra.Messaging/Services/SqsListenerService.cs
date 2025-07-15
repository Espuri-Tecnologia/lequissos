using Amazon.SQS;
using Amazon.SQS.Model;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Services
{
    public class SqsListenerService : BackgroundService
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<SqsListenerService> _logger;
        private readonly EventDispatcher _dispatcher;
        private readonly IReadOnlyList<string> _queueUrls;
        private readonly JsonSerializerOptions _jsonOptions;

        public SqsListenerService(IAmazonSQS sqsClient, IConfiguration configuration, ILogger<SqsListenerService> logger, EventDispatcher dispatcher)
        {
            _sqsClient = sqsClient;
            _logger = logger;
            var baseUrl = configuration["AWS:ServiceURL"]?.TrimEnd('/');
            var queuePaths = configuration.GetSection("AWS:SQSQueues").Get<string[]>() ?? Array.Empty<string>();
            _queueUrls = queuePaths.Select(q => $"{baseUrl}/{q.TrimStart('/')}").ToList();
            _dispatcher = dispatcher;
            _jsonOptions = new JsonSerializerOptions();
            _jsonOptions.Converters.Add(new BaseEventJsonConverter());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SQS Listener iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var queueUrl in _queueUrls)
                {
                    var request = new ReceiveMessageRequest
                    {
                        QueueUrl = queueUrl,
                        MaxNumberOfMessages = 10,
                        WaitTimeSeconds = 10
                    };

                    var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);

                    foreach (var message in response.Messages)
                    {
                        try
                        {
                            _logger.LogInformation($"Mensagem recebida: {message.Body}");

                            var typedEvent = JsonSerializer.Deserialize<BaseEvent>(message.Body, _jsonOptions)!;

                            await _dispatcher.DispatchAsync(typedEvent, stoppingToken);

                            await _sqsClient.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
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
}
