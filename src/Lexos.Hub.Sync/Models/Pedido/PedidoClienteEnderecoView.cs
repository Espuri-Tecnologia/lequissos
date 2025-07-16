namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoClienteEnderecoView
    {
        /// <summary>
        /// entrega/cobranca 
        /// </summary>
        public string TipoEndereco { get; set; }

        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Pais { get; set; }
        public string Cep { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
    }
}
