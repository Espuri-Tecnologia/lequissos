using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Response<PedidoDto>> MapPedidoAsync(PedidoView view);
    }
}
