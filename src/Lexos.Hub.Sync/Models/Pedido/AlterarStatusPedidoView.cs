namespace Lexos.Hub.Sync.Models.Pedido
{
    public class AlterarStatusPedidoView
    {
        public long IdPedido { get; set; }
        public string Status { get; set; }
        public StatusPedidoVendaView StatusPedidoVenda { get; set; }
    }

    public class StatusPedidoVendaView
    {
        public long? Id { get; set; }
        public string Nome { get; set; }
    }
}
