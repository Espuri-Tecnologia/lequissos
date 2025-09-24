using System;
using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.Hub.Sync.Models.Pedido;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public abstract class PedidoEventHandlerBase
    {
        private readonly ISqsRepository _syncInSqsRepository;
        private readonly string _queueNamePrefix = "notificacao-syncin-";

        protected PedidoEventHandlerBase(ISqsRepository syncInSqsRepository, IOptions<SyncInConfig> syncInSqsConfig)
        {
            _syncInSqsRepository = syncInSqsRepository ?? throw new ArgumentNullException(nameof(syncInSqsRepository));
            if (syncInSqsConfig == null) throw new ArgumentNullException(nameof(syncInSqsConfig));

            var config = syncInSqsConfig.Value ?? throw new ArgumentException("SyncIn configuration must be provided.");
            var queueUrl = $"{config.SQSBaseUrl}{config.SQSAccessKeyId}/{config.SQSName}";

            _syncInSqsRepository.IniciarFila(queueUrl);
        }

        protected void PublishPedidoRetorno(string hubKey, short plataformaId, PedidoRetornoView retorno)
        {
            if (string.IsNullOrWhiteSpace(hubKey)) throw new ArgumentException("Hub key is required.", nameof(hubKey));
            if (retorno == null) throw new ArgumentNullException(nameof(retorno));

            var notificacao = new NotificacaoAtualizacaoModel
            {
                Chave = hubKey,
                DataHora = DateTime.UtcNow,
                Json = JsonConvert.SerializeObject(retorno),
                TipoProcesso = TipoProcessoAtualizacao.Pedido,
                PlataformaId = plataformaId
            };

            _syncInSqsRepository.AdicionarMensagemFilaFifo(notificacao, $"{_queueNamePrefix}{hubKey}");
        }

        protected static PedidoRetornoView BuildPedidoRetornoView(
            PedidoView pedido,
            string? id,
            bool? pedidoIncluido = null,
            bool? pedidoAlterado = null,
            bool? pedidoCancelado = null,
            bool? pedidoIgnorado = null,
            bool erro = false,
            string? mensagem = null)
        {
            if (pedido == null) throw new ArgumentNullException(nameof(pedido));

            var statusId = pedido.PedidoStatusERPId.HasValue
                ? pedido.PedidoStatusERPId.Value > int.MaxValue
                    ? int.MaxValue
                    : (int)pedido.PedidoStatusERPId.Value
                : 0;

            return new PedidoRetornoView
            {
                PedidoId = pedido.PedidoId,
                PedidoERPId = !string.IsNullOrWhiteSpace(id) ? id! : pedido.PedidoERPId ?? string.Empty,
                PedidoERPStatusId = statusId,
                NomePlataformaERP = pedido.Plataforma ?? string.Empty,
                PedidoCancelado = pedidoCancelado ?? pedido.PedidoCancelado,
                PedidoIncluido = pedidoIncluido ?? false,
                PedidoAlterado = pedidoAlterado ?? false,
                PedidoIgnorado = pedidoIgnorado ?? false,
                Erro = erro,
                Mensagem = mensagem ?? string.Empty
            };
        }
    }
}
