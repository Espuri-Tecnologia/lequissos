namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoCampanhaView
    {
        public long PedidoId { get; set; }
        public bool InStore { get; set; }
        public bool SocialSelling { get; set; }
        public string VendedorId { get; set; }
    }
}
