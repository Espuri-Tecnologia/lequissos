using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;

namespace LexosHub.ERP.VarejOnline.Domain.Mappers
{
    public static class VarejoOnlinePedidoMapper
    {
        public static PedidoRequest Map(PedidoDto source)
        {
            return new PedidoRequest
            {
                Codigo = source.Codigo
            };
        }
    }
}
