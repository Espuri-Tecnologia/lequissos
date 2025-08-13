using System.Collections.Generic;
using System.Linq;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto
{
    public class ProdutoViewMapper
    {
        public ProdutoView? MapSimples(ProdutoResponse? source)
        {
            return ProdutoSimplesViewMapper.Map(source);
        }

        public List<ProdutoView> MapSimples(IEnumerable<ProdutoResponse>? source)
        {
            return source?.Select(MapSimples)
                .Where(v => v != null)
                .Cast<ProdutoView>()
                .ToList() ?? new List<ProdutoView>();
        }

        public ProdutoView? MapConfiguravel(ProdutoResponse produtoBase, List<ProdutoResponse> variacoes)
        {
            return ProdutoConfiguravelViewMapper.Map(produtoBase, variacoes);
        }

        public ProdutoView? MapKit(ProdutoResponse produtoBase)
        {
            var produto = ProdutoSimplesViewMapper.Map(produtoBase);

            if (produto == null)
            {
                return null;
            }

            produto.ProdutoTipoId = Lexos.Hub.Sync.Constantes.Produto.COMPOSTO;
            produto.Composicao = ProdutoKitViewMapper.Map(produtoBase.Componentes);

            return produto;
        }

        public List<ProdutoView> MapKits(IEnumerable<ProdutoResponse>? source)
        {
            return source?.Select(MapKit)
                .Where(v => v != null)
                .Cast<ProdutoView>()
                .ToList() ?? new List<ProdutoView>();
        }
    }
}
