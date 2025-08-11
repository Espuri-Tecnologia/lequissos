using AutoMapper;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejOnline.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IMapper _mapper;

        public PedidoService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<Response<PedidoDto>> MapPedidoAsync(PedidoView view)
        {
            var dto = _mapper.Map<PedidoDto>(view);
            return Task.FromResult(new Response<PedidoDto>(dto));
        }
    }
}
