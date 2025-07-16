using System;

namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoComposicaoPagamentoView
    {
        public string FormaPagamento { get; set; }
        public int QuantidadeParcelas { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
    }
}
