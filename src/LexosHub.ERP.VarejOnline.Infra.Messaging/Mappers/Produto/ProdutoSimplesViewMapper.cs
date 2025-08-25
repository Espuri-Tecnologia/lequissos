using Amazon.Auth.AccessControlPolicy;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

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

            if (source.Classificacao != null &&
                source.Classificacao.Equals("configuravel", StringComparison.OrdinalIgnoreCase) &&
                (source.ValorAtributos == null || !source.ValorAtributos.Any()))
            {
                return null;
            }

            decimal peso = GetNonNegative(source.Peso);
            decimal comprimento = GetNonNegative(source.Comprimento);
            decimal largura = GetNonNegative(source.Largura);
            decimal altura = GetNonNegative(source.Altura);

            return new ProdutoView
            {
                ProdutoTipoId = source.MercadoriaBase == true ? Lexos.Hub.Sync.Constantes.Produto.CONFIGURAVEL : Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
                Nome = Trim(source.Descricao, 255),
                Descricao = Trim(source.Descricao, 255),
                DescricaoMarketplace = Trim(source.Descricao, 255),
                DescricaoResumida = Trim(source.DescricaoSimplificada ?? $"{source.Descricao} - {source.CodigoSku}", 255),
                Ean = Trim(source.CodigoBarras, 50),
                Sku = Trim(source.CodigoSistema, 50),
                Peso = peso,
                Comprimento = comprimento,
                Largura = largura,
                Altura = altura,
                IsNew = true,
                Unidade = Trim(source.Unidade, 10),
                Deleted = !source.Ativo,
                Classificacao = source.Classificacao,
                Precos = source.MapToProdutoPrecoView(),
                Estoques = source.DadosPorEntidade?.MapEstoques(source.CodigoSistema!),
                ImagensCadastradas = source.MapImagensCadastradas(),
                Marca = source.Categorias?.FirstOrDefault(x => x.Nivel == "MARCA")?.Nome,
                Modelo = source.Categorias?.FirstOrDefault(x => x.Nivel == "COLEÇÃO")?.Nome,
                Setor = source.Categorias?.FirstOrDefault(x => x.Nivel == "DEPARTAMENTO")?.Nome,
                CategoriaERP = source.Categorias?.FirstOrDefault(x => x.Nivel == "TRIBUTAÇÃO")?.Nome,
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

        public static List<ProdutoEstoqueView> MapEstoques(this List<DadosPorEntidadeResponse> estoquesEntidades, string sku)
        {
            return estoquesEntidades.Select(estoque => estoque.MapEstoque(sku)).ToList();
        }

        public static ProdutoEstoqueView MapEstoque(this DadosPorEntidadeResponse estoqueEntidade, string sku)
        {
            return new ProdutoEstoqueView()
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
                        SkuVariacao = produto.CodigoSistema
                    });
                }
            }

            return new ProdutoImagemView
            {
                Imagens = imagens,
                Sku = produto.CodigoSku,
                TipoProdutoId = produto.MercadoriaBase == true ? Lexos.Hub.Sync.Constantes.Produto.CONFIGURAVEL : Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
            };
        }

        public static List<ProdutoImagemCadastradaView> MapImagensCadastradas(this ProdutoResponse produto)
        {
            if (produto.UrlsFotosProduto == null)
            {
                return new List<ProdutoImagemCadastradaView>();
            }

            return produto.UrlsFotosProduto.Select(u => new ProdutoImagemCadastradaView()
            {
                    Url = u,
                    Ordem = 0
            }).ToList();
        }

        public static List<ProdutoPrecoView> MapToProdutoPrecoView(this ProdutoResponse? produto, string produtoTipoId = Lexos.Hub.Sync.Constantes.Produto.SIMPLES)
        {
            List<ProdutoPrecoView> produtoPrecoViewList = new();

            if (produto.PrecosPorTabelas is null || !produto.PrecosPorTabelas.Any())
                return new List<ProdutoPrecoView> { 
                    new ProdutoPrecoView { 
                        Preco = produto.Preco.Value, 
                        Codigo = $"{CodigoProdutoPrecoTabela}{produto.Id}",
                        Sku = produto.CodigoSistema,
                    } 
                };

            foreach (TabelaPrecoResponse priceResponse in produto.PrecosPorTabelas)
                produtoPrecoViewList.Add(priceResponse.NewProdutoPrecoView(produto.CodigoSistema, produtoTipoId: produtoTipoId, codigo: CodigoProdutoPrecoTabela));

            return produtoPrecoViewList;
        }
        private static ProdutoPrecoView NewProdutoPrecoView(this TabelaPrecoResponse precoPorTabelaResponse, string sku, string produtoTipoId, string codigo)
        {
            ProdutoPrecoView produtoPrecoView = new()
            {
                Preco = precoPorTabelaResponse.Preco,
                Sku = sku,
                Codigo = $"{codigo}{precoPorTabelaResponse.IdTabelaPreco.ToString()}"
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
