using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Dispatcher
{
    public class SqslEventPublisher : ISqslEventPublisher
    {
        private readonly ISqsRepository _sqsRepository;
        private readonly Dictionary<string, string> _queueUrls;

        public SqslEventPublisher(
            ISqsRepository sqsRepository,
            IOptions<VarejOnlineSqsConfig> sqsVarejOnlineConfig)
        {
            _sqsRepository = sqsRepository ?? throw new ArgumentNullException(nameof(sqsRepository));

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
        }

        public async Task DispatchAsync(BaseEvent @event, CancellationToken cancellationToken)
        {
            if (@event is null) throw new ArgumentNullException(nameof(@event));

            var key = @event.EventType switch
            {
                nameof(CriarProdutosSimples) or nameof(CriarProdutosConfiguraveis) or nameof(CriarProdutosKits) => "Produtos",
                _ => @event.EventType
            };

            if (!_queueUrls.TryGetValue(key, out var queueUrl))
                throw new InvalidOperationException($"Nenhuma fila configurada para o evento '{@event.EventType}' (chave lógica '{key}').");

            _sqsRepository.IniciarFila(queueUrl, Lexos.SQS.Models.TipoConta.Production);

            var isFifo = queueUrl.EndsWith(".fifo", StringComparison.OrdinalIgnoreCase);

            var groupId =
                @event.GetType().GetProperty("HubKey")?.GetValue(@event)?.ToString()
                ?? @event.EventType
                ?? "default-group";

            dynamic dyn = @event;

            if (isFifo)
                _ = _sqsRepository.AdicionarMensagemFilaFifo(dyn, groupId);
            else
                _ = _sqsRepository.AdicionarMensagemFilaNormal(dyn);

            await Task.CompletedTask;
        }
    }
}
