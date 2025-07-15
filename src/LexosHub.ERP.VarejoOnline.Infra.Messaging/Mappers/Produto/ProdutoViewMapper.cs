using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoViewMapper
    {
        private const string CodigoProdutoPrecoVenda = "preco_venda";
        private const string CodigoProdutoPrefixoPrecoRegiao = "preco_regiao_";

        private static string? Trim(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static decimal GetNonNegative(decimal? value)
        {
            decimal result = value ?? 0m;
            return result < 0 ? 0 : result;
        }
        public static ProdutoView? Map(this ProdutoResponse? source)
        {
            if (source == null)
            {
                return null;
            }

            decimal peso = GetNonNegative(source.Peso);
            decimal comprimento = GetNonNegative(source.Comprimento);
            decimal largura = GetNonNegative(source.Largura);
            decimal altura = GetNonNegative(source.Altura);

            return new ProdutoView
            {
                ProdutoIdGlobal = source.Id,
                ProdutoId = source.Id,
                ProdutoTipoId = Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
                Nome = Trim(source.Descricao, 255),
                Descricao = Trim(source.Descricao, 255),
                DescricaoMarketplace = Trim(source.Descricao, 255),
                DescricaoResumida = Trim(source.DescricaoSimplificada ?? $"{source.Descricao} - {source.CodigoSku}", 255),
                Ean = Trim(source.CodigoBarras, 50),
                Sku = Trim(source.CodigoSku, 50),
                Peso = peso,
                Comprimento = comprimento,
                Largura = largura,
                Altura = altura,
                Unidade = Trim(source.Unidade, 10),
                Deleted = !source.Ativo,
                Classificacao = source.Classificacao,
                Precos = source.MapToProdutoPrecoView(),
                ImagensCadastradas = (source.UrlsFotosProduto ?? new List<string>()).MapImages(),
                Marca = source.Categorias?.FirstOrDefault(x => x.Nivel == "MARCA")?.Nome,
                Modelo = source.Categorias?.FirstOrDefault(x => x.Nivel == "COLEÇÃO")?.Nome,
                Setor = source.Categorias?.FirstOrDefault(x => x.Nivel == "DEPARTAMENTO")?.Nome,
                CategoriaERP = source.Categorias?.FirstOrDefault(x => x.Nivel == "TRIBUTAÇÃO")?.Nome,
                MetaTitle = Trim(source.Descricao, 60),
                MetaDescription = Trim(source.DescricaoSimplificada ?? source.Descricao, 160),
                Composicao = source.Componentes?.Select(c => new ProdutoComposicaoView
                {
                    ProdutoId = c.Produto.Id,
                    ProdutoIdGlobal = c.Produto.Id,
                    Quantidade = double.Parse(c.Quantidade.ToString())
                }).ToList() ?? new List<ProdutoComposicaoView>(),
                Categorias = source.Categorias?.Select(c => new ProdutoCategoriaView
                {
                    PlataformaId = 41
                }).ToList() ?? new List<ProdutoCategoriaView>(),
                Imagens = new ProdutoImagemView(),
                ProdutoEanComplemento = source.CodigosBarraAdicionais ?? new List<string>()
            };
        }

        public static List<ProdutoImagemCadastradaView> MapImages(this List<string> imagens)
        {
            if (imagens == null)
            {
                return new List<ProdutoImagemCadastradaView>();
            }

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
