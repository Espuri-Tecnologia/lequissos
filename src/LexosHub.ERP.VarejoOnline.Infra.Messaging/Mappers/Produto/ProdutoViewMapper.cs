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

            var variacoes = source.ValorAtributos?.Select(v => new ProdutoVariacaoView
            {
                Nome = v.Nome,
                Valor = v.Valor,
                Codigo = v.Codigo
            }).ToList() ?? new List<ProdutoVariacaoView>();

            // If product type is configuravel ensure at least one variation exists
            if (source.Classificacao == "configuravel" && !variacoes.Any())
            {
                return null;
            }

            return new ProdutoView
            {
                ProdutoIdGlobal = source.Id,
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
                Variacoes = variacoes,
                Composicao = source.Componentes?.Select(c => new ProdutoComponenteView
                {
                    ProdutoIdGlobal = c.Produto.Id,
                    Nome = c.Produto.Descricao,
                    Ean = c.Produto.CodigoBarras,
                    CodigoInterno = c.Produto.CodigoInterno,
                    CodigoSistema = c.Produto.CodigoSistema,
                    Quantidade = c.Quantidade,
                    Unidade = Trim(c.Unidade, 10)
                }).ToList() ?? new List<ProdutoComponenteView>(),
                Categorias = source.Categorias?.Select(c => new ProdutoCategoriaView
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Nivel = c.Nivel
                }).ToList() ?? new List<ProdutoCategoriaView>(),
                Imagens = source.UrlsFotosProduto?.Select(u => new ProdutoImagemView { Url = u }).ToList() ?? new List<ProdutoImagemView>(),
                ProdutoEanComplemento = source.CodigosBarraAdicionais ?? new List<string>(),
                ReferenciasOutrasPlataformas = new List<string>()
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
