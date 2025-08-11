using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using AlterarStatusResponse = LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses.AlterarStatusPedidoResponse;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<Response<PedidoResponse>> EnviarPedido(string hubKey, PedidoView pedidoView);
        Task<Response<AlterarStatusResponse>> AlterarStatusPedido(string hubKey, AlterarStatusPedidoView payload);
    }
}
