using Lexos.Hub.Sync.Models.Pedido;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class OrderCreated : BaseEvent
    {
        public string HubKey { get; set; } = null!;
        public PedidoView Pedido { get; set; } = null!;
    }
}
