using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Preco
{
    public static class TabelaPrecoMapper
    {
        private const string CodigoProdutoPrefixoPrecoRegiao = "preco_regiao_";

        public static ProdutoPrecoView? Map(this TabelaPrecoListResponse? source)
        {
            if (source == null)
                return null;

            return new ProdutoPrecoView
            {
                ProdutoId = source.Id,
                ProdutoIdGlobal = source.Id,
                Codigo = $"{CodigoProdutoPrefixoPrecoRegiao}{source.Id}",
                Descricao = source.Nome ?? string.Empty,
                Preco = 0,
                Tipo = Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
                Sku = string.Empty,
                Quantidade = 0
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
