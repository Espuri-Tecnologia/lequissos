namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido
{
    public class OrderShipped : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public long PedidoERPId { get; set; }
    }
}
