using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers
{
    public static class PriceTableMapper
    {
        public static ProdutoPrecoView? Map(this TabelaPrecoListResponse? source)
        {
            if (source == null)
                return null;

            return new ProdutoPrecoView
            {
                ProdutoId = source.Id,
                ProdutoIdGlobal = source.Id,
                Codigo = source.Id.ToString(),
                Descricao = source.Name ?? string.Empty,
                Preco = 0,
                Tipo = Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
                Sku = string.Empty,
                Quantidade = 0,
                TipoPrecoId = source.Id
            };
        }

        public static List<ProdutoPrecoView> Map(this List<TabelaPrecoListResponse>? source)
        {
            return source?.Select(Map)
                .Where(v => v != null)
                .Cast<ProdutoPrecoView>()
                .ToList() ?? new List<ProdutoPrecoView>();
        }
    }
}
