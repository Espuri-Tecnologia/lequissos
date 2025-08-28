using System;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Mappers
{
    public class VarejoOnlinePedidoMapperTests
    {
        [Fact]
        public void Map_ShouldMapTransporte_WhenInfoProvided()
        {
            var pedido = new PedidoView
            {
                Codigo = "1",
                Data = new DateTime(2024, 1, 1, 8, 0, 0),
                TipoFrete = "CIF",
                TransportadoraNome = "12345678000199"
            };

            var result = VarejoOnlinePedidoMapper.Map(pedido);

            Assert.NotNull(result);
            Assert.NotNull(result!.Transporte);
            Assert.Equal("CIF", result.Transporte!.Modalidade);
            Assert.NotNull(result.Transporte.Transportador);
            Assert.Equal("12345678000199", result.Transporte.Transportador!.Documento);
        }

        [Fact]
        public void Map_ShouldReturnNullTransporte_WhenNoInfo()
        {
            var pedido = new PedidoView
            {
                Codigo = "1",
                Data = new DateTime(2024, 1, 1)
            };

            var result = VarejoOnlinePedidoMapper.Map(pedido);

            Assert.NotNull(result);
            Assert.Null(result!.Transporte);
        }
    }
}
