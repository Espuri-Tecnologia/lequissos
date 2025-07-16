namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoNFeView
    {
        public string ChaveNFe { get; set; }
        public int LojaId { get; set; }
        public long LojaGlobalId { get; set; }
        public int Status { get; set; }
        public string LojaCNPJ { get; set; }
        public string ChaveIntegracao { get; set; }
    }
}
