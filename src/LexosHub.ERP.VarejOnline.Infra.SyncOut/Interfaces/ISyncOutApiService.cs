using Lexos.Hub.Sync.Models.Loja;
using Lexos.Log.Pedido.Models;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejOnline.Infra.SyncOut.Interfaces;

public interface ISyncOutApiService
{
    Task<Response<IntegracaoErpHubDto>> IntegrarLojas(IntegracaoErpHubDto integracaoRequest);
    Task<Response<bool>> AtualizarSituacaoImportacaoProdutos(string hubKey, bool iniciouExecucao);
    Task<Response<string>> RegistrarTimeline(string hubKey, NotificacaoTimelineView notificacaoTimeline);
}