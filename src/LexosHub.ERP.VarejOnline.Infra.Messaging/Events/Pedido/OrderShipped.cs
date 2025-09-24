using Lexos.Hub.Sync.Models.Pedido;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events.Pedido
{
    public class OrderShipped : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public long PedidoERPId { get; set; }
        public PedidoView Pedido { get; set; } = null!;
    }
}
