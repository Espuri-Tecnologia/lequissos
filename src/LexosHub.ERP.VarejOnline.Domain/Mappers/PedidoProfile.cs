using AutoMapper;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;

namespace LexosHub.ERP.VarejOnline.Domain.Mappers
{
    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<PedidoView, PedidoDto>();
        }
    }
}
