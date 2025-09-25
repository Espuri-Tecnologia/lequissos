namespace LexosHub.ERP.VarejOnline.Infra.SyncIn.Requests;

public class NotaFiscalExternaDto
{
    public string Chave { get; set; } = string.Empty;
    public long PedidoHubId { get; set; }
    public string XmlNota { get; set; } = string.Empty;
}