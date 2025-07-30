using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Mappers
{
    public class VarejOnlineProdutoMapperTests
    {
        [Fact]
        public void Map_ShouldHandleNullInput()
        {
            var result = VarejOnlineProdutoMapper.Map(null);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Map_ShouldMapFieldsCorrectly()
        {
            var source = new List<VarejOnlineProdutoDto>
            {
                new VarejOnlineProdutoDto
                {
                    id = 10,
                    descricao = "Produto X",
                    descricaoSimplificada = "Prod X",
                    codigoBarras = "123",
                    peso = 1.5m,
                    comprimento = 2.5m,
                    largura = 3.5m,
                    altura = 4.5m
                }
            };

            var result = VarejOnlineProdutoMapper.Map(source);

            Assert.Single(result);
            var item = result[0];
            Assert.Equal(10, item.ProdutoIdGlobal);
            Assert.Equal("Produto X", item.Nome);
            Assert.Equal("Prod X", item.DescricaoResumida);
            Assert.Equal("123", item.Ean);
            Assert.Equal(1.5m, item.Peso);
            Assert.Equal(2.5m, item.Comprimento);
            Assert.Equal(3.5m, item.Largura);
            Assert.Equal(4.5m, item.Altura);
        }
    }
}
