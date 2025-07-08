using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Xunit;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Mappers
{
    public class ProdutoViewMapperTests
    {
        [Fact]
        public void Map_ShouldReturnNull_WhenSourceIsNull()
        {
            var result = ProdutoViewMapper.Map((ProdutoResponse?)null);
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

            var result = ProdutoViewMapper.Map(source);

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
    }
}
