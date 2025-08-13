using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Mappers
{
    public class ProdutoKitViewMapperTests
    {
        [Fact]
        public void Map_ShouldNotThrow_WhenComponentProdutoIsNull()
        {
            var componentes = new List<ComponenteResponse>
            {
                new ComponenteResponse
                {
                    Produto = null!,
                    Quantidade = 2,
                    Unidade = "UN"
                }
            };

            var result = ProdutoKitViewMapper.Map(componentes);

            Assert.Single(result);
            var composicao = result[0];
            Assert.Equal(2, composicao.Quantidade);
            Assert.Equal(string.Empty, composicao.Sku);
        }
    }
}
