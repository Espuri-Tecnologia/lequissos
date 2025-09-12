using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Converters;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Services
{
    public class SqsListenerService : BackgroundService
    {
        private readonly ILogger<SqsListenerService> _logger;
        private readonly IEventDispatcher _dispatcher;
        private readonly Dictionary<string, string> _queueUrls; // key lógica -> URL completa
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ISqsRepository _sqsRepository;

        public SqsListenerService(
            ILogger<SqsListenerService> logger,
            IEventDispatcher dispatcher,
            IOptions<VarejOnlineSqsConfig> sqsVarejOnlineConfig,
            ISqsRepository sqsRepository)
        {
            _logger = logger;
            _dispatcher = dispatcher;
            _sqsRepository = sqsRepository;

            var cfg = sqsVarejOnlineConfig?.Value ?? throw new ArgumentNullException(nameof(sqsVarejOnlineConfig));
            if (string.IsNullOrWhiteSpace(cfg.SQSBaseUrl))
                throw new ArgumentException("SQSBaseUrl não configurado em AWSConfig.");
            if (string.IsNullOrWhiteSpace(cfg.SQSAccessKeyId))
                throw new ArgumentException("SQSAccessKeyId (AccountId) não configurado em AWSConfig.");
            if (cfg.SQSQueues is null || cfg.SQSQueues.Count == 0)
                throw new ArgumentException("SQSQueues vazio/não configurado em AWSConfig.");

            _queueUrls = cfg.SQSQueues.ToDictionary(
                kvp => kvp.Key,
                kvp => $"{cfg.SQSBaseUrl}{cfg.SQSAccessKeyId}/{kvp.Value}",
                StringComparer.OrdinalIgnoreCase
            );

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _jsonOptions.Converters.Add(new BaseEventJsonConverter());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SQS Listener iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Varre cada fila, uma por vez (repo é stateful)
                    foreach (var (logicalKey, queueUrl) in _queueUrls)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        _sqsRepository.IniciarFila(queueUrl, Lexos.SQS.Models.TipoConta.Production);

                        var msgs = _sqsRepository.ObterMensagens<JsonElement>(10);
                        foreach (var m in msgs ?? [])
                        {
                            var evt = JsonSerializer.Deserialize<BaseEvent>(m.Mensagem, _jsonOptions); // _jsonOptions tem BaseEventJsonConverter
                            if (evt is null) 
                                continue;

                            await _dispatcher.DispatchAsync(evt, stoppingToken);
                            _sqsRepository.RemoverMensagem(m.ReceiptHandle);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no loop principal do SQS Listener");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
