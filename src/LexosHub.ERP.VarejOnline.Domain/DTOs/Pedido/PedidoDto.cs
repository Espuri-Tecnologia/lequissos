namespace LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido
{
    /// <summary>
    /// Representa os dados mínimos de um pedido para integração com o Varejo Online.
    /// </summary>
    public class PedidoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string ClienteCpfCnpj { get; set; } = string.Empty;
        public string ClienteNome { get; set; } = string.Empty;
        public decimal? Total { get; set; }
    }
}
