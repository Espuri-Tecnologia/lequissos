namespace LexosHub.ERP.VarejOnline.Infra.SyncIn.Requests
{
    public class SyncInNotaFiscalDto
    {
        public int NFCeID { get; set; }
        public int Serie { get; set; }
        public string Chave { get; set; }
        public int NumeroNFCe { get; set; }
        public DateTime DataHoraEmissao { get; set; }
        public string XML { get; set; }
        public object XMLCancelamento { get; set; }
        public long EmpresaId { get; set; }
        public string Motivo { get; set; }
        public string Status { get; set; }
        public int tipoAmbiente { get; set; }
        public int numeroRecebimento { get; set; }
        public object codPDV { get; set; }
        public int TenantId { get; set; }
        public int NFCeIDNuvem { get; set; }
        public int ModeloDocumento { get; set; }
        public int Fluxo { get; set; }
        public int PedidoId { get; set; }
        public int LojaId { get; set; }
        public int LojaGlobalId { get; set; }
        public bool NotaExternaHub { get; set; }
    }
}
