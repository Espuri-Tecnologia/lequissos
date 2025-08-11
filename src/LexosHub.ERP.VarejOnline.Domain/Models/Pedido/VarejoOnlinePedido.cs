namespace Lexos.Hub.Sync.Models.Pedido
{
    /// <summary>
    /// Modelo de pedido utilizado pelo domínio para integração com o Varejo Online.
    /// </summary>
    public class VarejoOnlinePedido
    {
        public string Codigo { get; set; } = string.Empty;
        public string ClienteCpfCnpj { get; set; } = string.Empty;
        public string ClienteNome { get; set; } = string.Empty;
        public decimal? Total { get; set; }
    }
}
