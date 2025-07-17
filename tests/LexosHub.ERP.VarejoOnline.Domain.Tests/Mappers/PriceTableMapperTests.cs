using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;
using Xunit;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Mappers
{
    public class PriceTableMapperTests
    {
        [Fact]
        public void Map_ShouldReturnNull_WhenSourceIsNull()
        {
            var result = PriceTableMapper.Map((TabelaPrecoListResponse?)null);
            Assert.Null(result);
        }

        [Fact]
        public void MapList_ShouldMapFields()
        {
            var source = new List<TabelaPrecoListResponse>
            {
                new TabelaPrecoListResponse
                {
                    Id = 1,
                    Name = "Tabela",
                    IsActive = true
                }
            };

            var result = source.Map();

            Assert.Single(result);
            var item = result[0];
            Assert.Equal(1, item.ProdutoId);
            Assert.Equal(1, item.ProdutoIdGlobal);
            Assert.Equal("1", item.Codigo);
            Assert.Equal("Tabela", item.Descricao);
            Assert.Equal(1, item.TipoPrecoId);
        }
    }
}
