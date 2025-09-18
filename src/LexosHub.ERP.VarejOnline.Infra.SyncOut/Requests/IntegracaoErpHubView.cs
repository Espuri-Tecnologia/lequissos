using Lexos.Hub.Sync.Models.Loja;

namespace LexosHub.ERP.VarejOnline.Infra.SyncOut.Requests;

public class IntegracaoErpHubView
{
    public List<LojaDto>? Lojas { get; set; }
    public string? Chave { get; set; }
    public string? Retorno { get; set; }
}