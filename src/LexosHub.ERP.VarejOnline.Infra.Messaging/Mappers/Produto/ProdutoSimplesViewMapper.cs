using System;
using System.Collections.Generic;
using System.Linq;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoSimplesViewMapper
    {
        private const string CodigoProdutoPrecoTabela = "preco_regiao_";

        private static string? Trim(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static decimal GetNonNegative(decimal? value) => Math.Max(value ?? 0m, 0m);
        public static ProdutoView? Map(this ProdutoResponse? source)
        {
            if (source == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(source.Classificacao) &&
                source.Classificacao.Equals("configuravel", StringComparison.OrdinalIgnoreCase) &&
                !(source.ValorAtributos?.Any() ?? false))
            {
                return null;
            }

            var sku = Trim(source.CodigoSistema, 50) ?? Trim(source.CodigoSku, 50) ?? string.Empty;
            var descricao = Trim(source.Descricao, 255);
            var descricaoSimplificada = Trim(source.DescricaoSimplificada, 255);

            return new ProdutoView
            {
                ProdutoIdGlobal = source.Id,
                ProdutoTipoId = source.MercadoriaBase == true
                    ? Lexos.Hub.Sync.Constantes.Produto.CONFIGURAVEL
                    : Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
                Nome = descricao,
                Descricao = descricao,
                DescricaoMarketplace = descricao,
                DescricaoResumida = descricaoSimplificada ?? Trim($"{source.Descricao} - {source.CodigoSku}", 255),
                Ean = Trim(source.CodigoBarras, 50),
                Sku = sku,
                Peso = GetNonNegative(source.Peso),
                Comprimento = GetNonNegative(source.Comprimento),
                Largura = GetNonNegative(source.Largura),
                Altura = GetNonNegative(source.Altura),
                IsNew = true,
                Unidade = Trim(source.Unidade, 10),
                Deleted = !source.Ativo,
                Classificacao = source.Classificacao,
                Precos = source.MapToProdutoPrecoView(),
                Estoques = source.DadosPorEntidade.MapEstoques(sku),
                ImagensCadastradas = source.MapImagensCadastradas(),
                Marca = GetCategoriaNome(source.Categorias, "MARCA"),
                Modelo = GetCategoriaNome(source.Categorias, "COLEÇÃO"),
                Setor = GetCategoriaNome(source.Categorias, "DEPARTAMENTO"),
                CategoriaERP = GetCategoriaNome(source.Categorias, "TRIBUTAÇÃO"),
                MetaTitle = Trim(source.Descricao, 60),
                MetaDescription = Trim(source.DescricaoSimplificada ?? source.Descricao, 160),
                Composicao = ProdutoKitViewMapper.Map(source.Componentes),
                ProdutoEanComplemento = source.CodigosBarraAdicionais ?? new List<string>(),
                ProdutoImposto = new ProdutoImpostoView
                {
                    NCM = source.CodigoNcm,
                    Origem = source.Origem.HasValue ? (short)source.Origem.Value : short.MinValue,
                    Cest = source.CodigoCest
                }
            };
        }

        private static string? GetCategoriaNome(IEnumerable<CategoriaResponse>? categorias, string nivel)
        {
            return categorias?
                .FirstOrDefault(x => string.Equals(x?.Nivel, nivel, StringComparison.OrdinalIgnoreCase))?
                .Nome;
        }

        public static List<ProdutoEstoqueView> MapEstoques(this IEnumerable<DadosPorEntidadeResponse>? estoquesEntidades, string sku)
        {
            if (estoquesEntidades == null)
            {
                return new List<ProdutoEstoqueView>();
            }

            return estoquesEntidades
                .Select(estoque => estoque.MapEstoque(sku))
                .ToList();
        }

        public static ProdutoEstoqueView MapEstoque(this DadosPorEntidadeResponse? estoqueEntidade, string sku)
        {
            if (estoqueEntidade == null)
            {
                return new ProdutoEstoqueView
                {
                    LojaIdGlobal = 0,
                    Sku = sku,
                    Quantidade = 0
                };
            }

            return new ProdutoEstoqueView
            {
                LojaIdGlobal = (int)estoqueEntidade.Entidade,
                Sku = sku,
                Quantidade = estoqueEntidade.EstoqueMaximo
            };
        }

        public static ProdutoImagemView MapImages(this ProdutoResponse produto)
        {
            var imagens = new List<ImportJobImagemView>();

            if (produto.UrlsFotosProduto != null && produto.UrlsFotosProduto.Any())
            {
                for (int i = 0; i < produto.UrlsFotosProduto.Count; i++)
                {
                    imagens.Add(new ImportJobImagemView
                    {
                        Url = produto.UrlsFotosProduto[i],
                        IsThumbnail = (i == 0),
                        Position = (short)(i + 1),
                        SkuVariacao = produto.CodigoSistema ?? string.Empty
                    });
                }
            }

            return new ProdutoImagemView
            {
                Imagens = imagens,
                Sku = produto.CodigoSku,
                TipoProdutoId = produto.MercadoriaBase == true
                    ? Lexos.Hub.Sync.Constantes.Produto.CONFIGURAVEL
                    : Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
            };
        }

        public static List<ProdutoImagemCadastradaView> MapImagensCadastradas(this ProdutoResponse? produto)
        {
            if (produto?.UrlsFotosProduto == null)
            {
                return new List<ProdutoImagemCadastradaView>();
            }

            return produto.UrlsFotosProduto
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(u => new ProdutoImagemCadastradaView
                {
                    Url = u,
                    Ordem = 0
                })
                .ToList();
        }

        public static List<ProdutoPrecoView> MapToProdutoPrecoView(this ProdutoResponse? produto, string produtoTipoId = Lexos.Hub.Sync.Constantes.Produto.SIMPLES)
        {
            if (produto == null)
            {
                return new List<ProdutoPrecoView>();
            }

            var sku = produto.CodigoSistema ?? string.Empty;

            if (produto.PrecosPorTabelas == null || !produto.PrecosPorTabelas.Any())
            {
                if (!produto.Preco.HasValue)
                {
                    return new List<ProdutoPrecoView>();
                }

                return new List<ProdutoPrecoView>
                {
                    new ProdutoPrecoView
                    {
                        Preco = GetNonNegative(produto.Preco),
                        Codigo = $"{CodigoProdutoPrecoTabela}{produto.Id}",
                        Sku = sku
                    }
                };
            }

            return produto.PrecosPorTabelas
                .Where(priceResponse => priceResponse != null)
                .Select(priceResponse => priceResponse.NewProdutoPrecoView(sku, produtoTipoId, CodigoProdutoPrecoTabela))
                .ToList();
        }
        private static ProdutoPrecoView NewProdutoPrecoView(this TabelaPrecoResponse precoPorTabelaResponse, string sku, string produtoTipoId, string codigo)
        {
            return new ProdutoPrecoView
            {
                Preco = GetNonNegative(precoPorTabelaResponse?.Preco),
                Sku = sku,
                Codigo = $"{codigo}{precoPorTabelaResponse?.IdTabelaPreco.ToString()}"
            };
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
