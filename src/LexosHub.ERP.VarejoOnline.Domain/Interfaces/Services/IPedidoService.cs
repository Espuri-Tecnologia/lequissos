using LexosHub.ERP.VarejoOnline.Domain.DTOs.Pedido;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task ProcessWebhookAsync(PedidoWebhookDto pedido, CancellationToken cancellationToken = default);
    }
}
