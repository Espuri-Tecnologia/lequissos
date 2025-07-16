using System.Collections.Generic;

namespace Lexos.Hub.Sync.Models.Pedido
{
    public class ReprocessarPedidoPlataformaDto
    {
        public List<DadosPedidoDto> Pedidos { get; set; }
        public int TenantId { get; set; }
        public int IntegracaoId { get; set; }
    }
}
