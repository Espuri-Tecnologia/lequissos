using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Requests;

namespace LexosHub.ERP.VarejOnline.Infra.SyncIn.Interfaces;

public interface ISyncInApiService
{
    Task<Response<NotaFiscalExternaDto>> InserirNotaFiscalExterna(NotaFiscalExternaDto nfeExternal);
    Task<Response<SyncInNotaFiscalDto>> ObterNotaFiscalPorChave(string chaveNotaFiscal, string chave);
} 