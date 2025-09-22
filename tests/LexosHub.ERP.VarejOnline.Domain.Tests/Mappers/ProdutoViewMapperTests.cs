using System;
using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Mappers
{
    public class ProdutoViewMapperTests
    {
        private readonly ProdutoViewMapper _mapper = new();

        private static ProdutoResponse CreateProdutoResponse(Action<ProdutoResponse>? configure = null)
        {
            var response = new ProdutoResponse
            {
                Id = 1,
                Descricao = "Produto",
                CodigoSku = "SKU",
                CodigoSistema = "SKU",
                Ativo = true,
                Preco = 10m,
                DadosPorEntidade = new List<DadosPorEntidadeResponse>(),
                Categorias = new List<CategoriaResponse>(),
                ValorAtributos = new List<ValorAtributoResponse>()
            };

            configure?.Invoke(response);
            return response;
        }

        [Fact]
        public void Map_ShouldReturnNull_WhenSourceIsNull()
        {
            var result = _mapper.MapSimples((ProdutoResponse?)null);
            Assert.Null(result);
        }

        [Fact]
        public void MapList_ShouldMapFields()
        {
            var source = new List<ProdutoResponse>
            {
                CreateProdutoResponse(p =>
                {
                    p.Id = 5;
                    p.Descricao = "Produto";
                    p.DescricaoSimplificada = "Prod";
                    p.CodigoBarras = "111";
                    p.Peso = 1.1m;
                    p.Comprimento = 2.2m;
                    p.Largura = 3.3m;
                    p.Altura = 4.4m;
                })
            };

            var result = _mapper.MapSimples(source);

            Assert.Single(result);
            var item = result[0];
            Assert.Equal(5, item.ProdutoIdGlobal);
            Assert.Equal("Produto", item.Nome);
            Assert.Equal("Prod", item.DescricaoResumida);
            Assert.Equal("111", item.Ean);
            Assert.Equal(1.1m, item.Peso);
            Assert.Equal(2.2m, item.Comprimento);
            Assert.Equal(3.3m, item.Largura);
            Assert.Equal(4.4m, item.Altura);
        }

        [Fact]
        public void Map_ShouldInitializeLists_WhenSourceListsAreNull()
        {
            var source = CreateProdutoResponse(p =>
            {
                p.Preco = null;
                p.CodigosBarraAdicionais = null;
                p.DadosPorEntidade = null;
                p.Categorias = null;
                p.Componentes = null;
                p.PrecosPorTabelas = null;
            });

            var result = _mapper.MapSimples(source)!;

            Assert.NotNull(result.Precos);
            Assert.Empty(result.Precos);
            Assert.NotNull(result.ImagensCadastradas);
            Assert.Empty(result.ImagensCadastradas);
            Assert.NotNull(result.Variacoes);
            Assert.Empty(result.Variacoes);
            Assert.NotNull(result.Composicao);
            Assert.Empty(result.Composicao);
            Assert.NotNull(result.Categorias);
            Assert.Empty(result.Categorias);
            Assert.NotNull(result.Imagens);
            //Assert.Empty(result.Imagens);
            Assert.NotNull(result.ProdutoEanComplemento);
            Assert.Empty(result.ProdutoEanComplemento);
            Assert.NotNull(result.ReferenciasOutrasPlataformas);
            Assert.Empty(result.ReferenciasOutrasPlataformas);
        }

        [Fact]
        public void Map_ShouldTrimSkuAndLimitLength()
        {
            var sku = new string('a', 55);
            var source = CreateProdutoResponse(p =>
            {
                p.Id = 2;
                p.Descricao = "Desc";
                p.CodigoSistema = null;
                p.CodigoSku = "  " + sku + "  ";
            });

            var result = _mapper.MapSimples(source)!;

            Assert.Equal(50, result.Sku!.Length);
            Assert.False(result.Sku.StartsWith(" "));
            Assert.False(result.Sku.EndsWith(" "));
        }

        [Fact]
        public void Map_ShouldHandleNullAndNegativeNumericFields()
        {
            var source = CreateProdutoResponse(p =>
            {
                p.Id = 3;
                p.Descricao = "P";
                p.CodigoSku = "S";
                p.Peso = -1;
                p.Altura = null;
                p.Comprimento = -2;
                p.Largura = null;
            });

            var result = _mapper.MapSimples(source)!;

            Assert.Equal(0, result.Peso);
            Assert.Equal(0, result.Altura);
            Assert.Equal(0, result.Comprimento);
            Assert.Equal(0, result.Largura);
        }

        [Fact]
        public void Map_ShouldReturnNull_WhenConfiguravelWithoutVariacoes()
        {
            var source = CreateProdutoResponse(p =>
            {
                p.Id = 4;
                p.Descricao = "Produto";
                p.CodigoSku = "SKU";
                p.Classificacao = "configuravel";
                p.ValorAtributos = new List<ValorAtributoResponse>();
            });

            var result = _mapper.MapSimples(source);

            Assert.Null(result);
        }

        [Fact]
        public void Map_ShouldRespectStringLengthLimits()
        {
            var longName = new string('n', 300);
            var longEan = new string('e', 60);
            var longDesc = new string('d', 300);

            var source = CreateProdutoResponse(p =>
            {
                p.Id = 5;
                p.Descricao = longName;
                p.DescricaoSimplificada = longDesc;
                p.CodigoSku = "sku";
                p.CodigoBarras = longEan;
            });

            var result = _mapper.MapSimples(source)!;

            Assert.True(result.Nome!.Length <= 255);
            Assert.True(result.DescricaoResumida!.Length <= 255);
            Assert.True(result.Ean!.Length <= 50);
        }

        [Fact]
        public void Map_ShouldHandleNullCategorias()
        {
            var source = CreateProdutoResponse(p =>
            {
                p.Id = 6;
                p.Descricao = "Prod";
                p.CodigoSku = "S";
                p.Categorias = null;
            });

            var result = _mapper.MapSimples(source)!;

            Assert.Null(result.Marca);
            Assert.Empty(result.Categorias);
        }

        [Fact]
        public void Map_ShouldMapDefaultValuesWhenPropertiesMissing()
        {
            var source = CreateProdutoResponse(p =>
            {
                p.Id = 7;
                p.Descricao = "  Produto  ";
                p.CodigoSistema = null;
                p.CodigoSku = "  SKU  ";
                p.Preco = null;
            });

            var result = _mapper.MapSimples(source)!;

            Assert.Equal("Produto", result.Nome);
            Assert.Equal("SKU", result.Sku);
            Assert.Null(result.Ean);
            Assert.Equal(0, result.Peso);
            Assert.Equal(0, result.Comprimento);
            Assert.Equal(0, result.Largura);
            Assert.Equal(0, result.Altura);
        }
    

        [Fact]
        public void MapConfiguravel_ShouldMapVariacoes()
        {
            var produtoBase = new ProdutoResponse
            {
                Id = 10,
                Descricao = "Produto Base",
                CodigoSku = "BASE",
                CodigoSistema = "BASE",
                MercadoriaBase = true,
            };

            var variacoes = new List<ProdutoResponse>
            {
                new ProdutoResponse
                {
                    Id = 11,
                    CodigoSistema = "VAR1",
                    CodigoBarras = "EAN1",
                    Ativo = true,
                    ValorAtributos = new List<ValorAtributoResponse>
                    {
                        new ValorAtributoResponse { Nome = "TAMANHO", Valor = "M" },
                        new ValorAtributoResponse { Nome = "COR", Valor = "Azul" }
                    }
                },
                new ProdutoResponse
                {
                    Id = 12,
                    CodigoSistema = "VAR2",
                    CodigoBarras = "EAN2",
                    Ativo = false,
                    ValorAtributos = new List<ValorAtributoResponse>
                    {
                        new ValorAtributoResponse { Nome = "TAMANHO", Valor = "G" },
                        new ValorAtributoResponse { Nome = "COR", Valor = "Vermelho" }
                    }
                }
            };

            var result = _mapper.MapConfiguravel(produtoBase, variacoes)!;

            Assert.Equal(Lexos.Hub.Sync.Constantes.Produto.CONFIGURAVEL, result.ProdutoTipoId);
            Assert.Equal(2, result.Variacoes.Count);

            var v1 = result.Variacoes[0];
            Assert.Equal("VAR1", v1.Sku);
            Assert.Equal("EAN1", v1.EAN);
            Assert.Equal("M", v1.Tamanho);
            Assert.Equal("Azul", v1.Cor);
            Assert.True(v1.Deleted == false);

            var v2 = result.Variacoes[1];
            Assert.Equal("VAR2", v2.Sku);
            Assert.Equal("EAN2", v2.EAN);
            Assert.Equal("G", v2.Tamanho);
            Assert.Equal("Vermelho", v2.Cor);
            Assert.True(v2.Deleted);
        }

        [Fact]
        public void MapKit_ShouldMapComposicaoAndSetTipo()
        {
            var produtoBase = new ProdutoResponse
            {
                Id = 20,
                Descricao = "Kit Produto",
                CodigoSku = "KIT",
                Componentes = new List<ComponenteResponse>
                {
                    new ComponenteResponse
                    {
                        Quantidade = 2,
                        Unidade = "UN",
                        Produto = new ComponenteProdutoResponse
                        {
                            Id = 30,
                            CodigoSistema = "SKU1"
                        }
                    },
                    new ComponenteResponse
                    {
                        Quantidade = 1,
                        Unidade = "UN",
                        Produto = new ComponenteProdutoResponse
                        {
                            Id = 31,
                            CodigoSistema = "SKU2"
                        }
                    }
                }
            };

            var result = _mapper.MapKit(produtoBase)!;

            Assert.Equal(Lexos.Hub.Sync.Constantes.Produto.COMPOSTO, result.ProdutoTipoId);
            Assert.Equal(2, result.Composicao.Count);

            var c1 = result.Composicao[0];
            Assert.Equal(30, c1.ProdutoId);
            Assert.Equal("SKU1", c1.Sku);
            Assert.Equal(2, c1.Quantidade);

            var c2 = result.Composicao[1];
            Assert.Equal(31, c2.ProdutoId);
            Assert.Equal("SKU2", c2.Sku);
            Assert.Equal(1, c2.Quantidade);
        }
    }
}
