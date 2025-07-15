using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;

namespace LexosHub.ERP.VarejoOnline.Domain.DTOs.Pedido
{
    public class PedidoWebhookDto
    {
        public string HubKey { get; set; } = string.Empty;
        public List<ProdutoResponse>? Produtos { get; set; }
    }
}
