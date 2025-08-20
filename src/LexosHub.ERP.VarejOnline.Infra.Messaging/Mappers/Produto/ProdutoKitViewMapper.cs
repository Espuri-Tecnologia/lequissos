using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoKitViewMapper
    {
        public static List<ProdutoComposicaoView> Map(List<ComponenteResponse>? componentes)
        {
            return componentes?.Select(MapComponente).ToList() ?? new List<ProdutoComposicaoView>();
        }

        private static ProdutoComposicaoView MapComponente(ComponenteResponse c)
        {
            return new ProdutoComposicaoView
            {
                Quantidade = (double)c.Quantidade,
                Sku = c.Produto?.CodigoSistema ?? string.Empty
            };
        }
    }
}
