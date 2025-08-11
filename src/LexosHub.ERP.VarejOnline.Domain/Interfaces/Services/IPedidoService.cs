using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Response<PedidoResponse>> CreateAsync(PedidoDto dto);
    }
}
