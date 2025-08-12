using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using System.Collections.Generic;
using System.Linq;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoKitViewMapper
    {
        public static ProdutoView? Map(ProdutoResponse produtoBase)
        {
            if (produtoBase == null)
            {
                return null;
            }

            var produtoView = ProdutoSimplesViewMapper.Map(produtoBase);
            if (produtoView == null)
            {
                return null;
            }

            produtoView.ProdutoTipoId = Lexos.Hub.Sync.Constantes.Produto.COMPOSTO;
            produtoView.Composicao = produtoBase.Componentes?.Select(c => new ProdutoComposicaoView
            {
                ProdutoIdGlobal = c.Produto.Id,
                Sku = c.Produto.CodigoSistema,
                Quantidade = double.Parse(c.Quantidade.ToString())
            }).ToList() ?? new List<ProdutoComposicaoView>();

            return produtoView;
        }
    }
}
