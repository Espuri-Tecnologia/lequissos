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
                new ProdutoResponse
                {
                    Id = 5,
                    Descricao = "Produto",
                    DescricaoSimplificada = "Prod",
                    CodigoBarras = "111",
                    Peso = 1.1m,
                    Comprimento = 2.2m,
                    Largura = 3.3m,
                    Altura = 4.4m
                }
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
            var source = new ProdutoResponse
            {
                Id = 1,
                Descricao = "Produto",
                CodigoSku = "SKU"
            };

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
            var source = new ProdutoResponse
            {
                Id = 2,
                Descricao = "Desc",
                CodigoSku = "  " + sku + "  "
            };

            var result = _mapper.MapSimples(source)!;

            Assert.Equal(50, result.Sku!.Length);
            Assert.False(result.Sku.StartsWith(" "));
            Assert.False(result.Sku.EndsWith(" "));
        }

        [Fact]
        public void Map_ShouldHandleNullAndNegativeNumericFields()
        {
            var source = new ProdutoResponse
            {
                Id = 3,
                Descricao = "P",
                CodigoSku = "S",
                Peso = -1,
                Altura = null,
                Comprimento = -2,
                Largura = null
            };

            var result = _mapper.MapSimples(source)!;

            Assert.Equal(0, result.Peso);
            Assert.Equal(0, result.Altura);
            Assert.Equal(0, result.Comprimento);
            Assert.Equal(0, result.Largura);
        }

        [Fact]
        public void Map_ShouldReturnNull_WhenConfiguravelWithoutVariacoes()
        {
            var source = new ProdutoResponse
            {
                Id = 4,
                Descricao = "Produto",
                CodigoSku = "SKU",
                Classificacao = "configuravel",
                ValorAtributos = new List<ValorAtributoResponse>()
            };

            var result = _mapper.MapSimples(source);

            Assert.Null(result);
        }

        [Fact]
        public void Map_ShouldRespectStringLengthLimits()
        {
            var longName = new string('n', 300);
            var longEan = new string('e', 60);
            var longDesc = new string('d', 300);

            var source = new ProdutoResponse
            {
                Id = 5,
                Descricao = longName,
                DescricaoSimplificada = longDesc,
                CodigoSku = "sku",
                CodigoBarras = longEan
            };

            var result = _mapper.MapSimples(source)!;

            Assert.True(result.Nome!.Length <= 255);
            Assert.True(result.DescricaoResumida!.Length <= 255);
            Assert.True(result.Ean!.Length <= 50);
        }

        [Fact]
        public void Map_ShouldHandleNullCategorias()
        {
            var source = new ProdutoResponse
            {
                Id = 6,
                Descricao = "Prod",
                CodigoSku = "S",
                Categorias = null
            };

            var result = _mapper.MapSimples(source)!;

            Assert.Null(result.Marca);
            Assert.Empty(result.Categorias);
        }

        [Fact]
        public void Map_ShouldMapDefaultValuesWhenPropertiesMissing()
        {
            var source = new ProdutoResponse
            {
                Id = 7,
                Descricao = "  Produto  ",
                CodigoSku = "  SKU  "
            };

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
    }
}
