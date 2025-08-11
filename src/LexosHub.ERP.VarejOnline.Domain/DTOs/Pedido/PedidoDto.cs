namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido
{
    public class PedidoDto
    {
        public long PedidoId { get; set; }
        public string? Codigo { get; set; }
        public decimal? Total { get; set; }
    }
}
