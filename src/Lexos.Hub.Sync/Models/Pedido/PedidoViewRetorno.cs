namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoRetornoView
    {
        public long PedidoId { get; set; }

        public string PedidoERPId { get; set; }
        public string PedidoERPNumero { get; set; }
        public int PedidoERPStatusId { get; set; }
        public string PedidoERPStatus { get; set; }

        public string NomePlataformaERP { get; set; }

        public bool PedidoCancelado { get; set; }
        public bool PedidoIncluido { get; set; }
        public bool PedidoAlterado { get; set; }
        public bool PedidoIgnorado { get; set; }
        public bool Erro { get; set; }
        public string Mensagem { get; set; }
    }
}
