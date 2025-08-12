using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;

namespace LexosHub.ERP.VarejOnline.Domain.Mappers
{
    /// <summary>
    /// Realiza o mapeamento de <see cref="PedidoView"/> para <see cref="PedidoRequest"/>.
    /// </summary>
    public static class VarejoOnlinePedidoMapper
    {
        /// <summary>
        /// Converte os dados do <see cref="PedidoView"/> para um <see cref="PedidoRequest"/>.
        /// </summary>
        /// <param name="source">Instância de origem.</param>
        /// <returns>Objeto <see cref="PedidoRequest"/> com as informações necessárias.</returns>
        public static PedidoRequest? Map(PedidoView? source)
        {
            if (source == null)
            {
                return null;
            }

            return new PedidoRequest
            {
            };
        }
    }
}
