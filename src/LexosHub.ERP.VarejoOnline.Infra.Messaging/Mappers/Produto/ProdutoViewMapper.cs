using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoViewMapper
    {
        private const string CodigoProdutoPrecoVenda = "preco_venda";
        private const string CodigoProdutoPrefixoPrecoRegiao = "preco_regiao_";
        public static ProdutoView? Map(this ProdutoResponse? source)
        {
            return source == null ? null : new ProdutoView
            {
                ProdutoIdGlobal = source.Id,
                Nome = source.Descricao,
                Descricao = source.Descricao,
                DescricaoMarketplace = source.Descricao,
                DescricaoResumida = $"{source.Descricao} - {source.CodigoSku}",
                Ean = source.CodigoBarras,
                Sku = source.CodigoSku,
                Peso = source.Peso ?? source.Peso.Value,
                Comprimento = source.Comprimento.HasValue ? source.Comprimento.Value : 0,
                Largura = source.Largura.HasValue ? source.Largura.Value : 0,
                Altura = source.Altura.HasValue ? source.Altura.Value : 0,
                Unidade = source.Unidade,
                Deleted = !source.Ativo,
                Classificacao = source.Classificacao,
                Precos = source.MapToProdutoPrecoView(),
                ImagensCadastradas = source.UrlsFotosProduto?.MapImages(),
                Marca = source.Categorias.FirstOrDefault(x => x.Nivel == "MARCA")?.Nome,
                Modelo = source.Categorias.FirstOrDefault(x => x.Nivel == "COLEÇÃO")?.Nome,
                Setor = source.Categorias.FirstOrDefault(x => x.Nivel == "DEPARTAMENTO")?.Nome,
                CategoriaERP = source.Categorias.FirstOrDefault(x => x.Nivel == "TRIBUTAÇÃO")?.Nome
            };
        }

        public static List<ProdutoImagemCadastradaView> MapImages(this List<string> imagens)
        {
            return imagens.Select(u => new ProdutoImagemCadastradaView { Url = u }).ToList();
        }

        public static List<ProdutoPrecoView> MapToProdutoPrecoView(this ProdutoResponse? produto, string produtoTipoId = Lexos.Hub.Sync.Constantes.Produto.SIMPLES)
        {
            List<ProdutoPrecoView> produtoPrecoViewList = new();

            if (produto.PrecosPorTabelas is null || !produto.PrecosPorTabelas.Any())
                return new List<ProdutoPrecoView>();

            foreach (PrecoPorTabelaResponse priceResponse in produto.PrecosPorTabelas)
                produtoPrecoViewList.Add(priceResponse.NewProdutoPrecoView(produto.CodigoSku, produtoTipoId: produtoTipoId, codigo: CodigoProdutoPrecoVenda));

            return produtoPrecoViewList;
        }
        private static ProdutoPrecoView NewProdutoPrecoView(this PrecoPorTabelaResponse precoPorTabelaResponse, string sku, string produtoTipoId, string codigo)
        {
            ProdutoPrecoView produtoPrecoView = new()
            {
                Preco = precoPorTabelaResponse.Preco,
                Descricao = $"Preco Sku: {sku}",
                Sku = sku,
                Tipo = produtoTipoId,
                Codigo = codigo
            };
            return produtoPrecoView;
        }


        public static List<ProdutoView> Map(this List<ProdutoResponse> source)
        {
            return source?.Select(Map)
                .Where(v => v != null)
                .Cast<ProdutoView>()
                .ToList() ?? new List<ProdutoView>();
        }
    }
}
