using System.Collections.Generic;
using System.Linq;
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
                ProdutoId = c.Produto?.Id ?? 0,
                ProdutoIdGlobal = c.Produto?.Id ?? 0,
                ProdutoCompostoId = c.Produto?.Id ?? 0,
                ProdutoCompostoIdGlobal = c.Produto?.Id ?? 0,
                Quantidade = (double)c.Quantidade,
                Tipo = c.Unidade ?? string.Empty,
                Sku = c.Produto?.CodigoSistema ?? string.Empty
            };
        }
    }
}
