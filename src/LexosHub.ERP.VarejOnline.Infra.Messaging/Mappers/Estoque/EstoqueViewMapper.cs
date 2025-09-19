using Lexos.Hub.Sync.Models.Produto;
using Lexos.Hub.Sync;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using System.Globalization;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Estoque
{
    public static class EstoqueViewMapper
    {
        public static List<ProdutoEstoqueView> Map(IEnumerable<EstoqueResponse>? source)
        {
            if (source == null)
            {
                return new List<ProdutoEstoqueView>();
            }

            return source
                .Where(item => item is not null)
                .Select(MapItem)
                .ToList();
        }

        private static ProdutoEstoqueView MapItem(EstoqueResponse source)
        {
            var produtoId = source.Produto?.Id ?? 0;
            var entidade = source.Entidade?.Id ?? 0;

            return new ProdutoEstoqueView
            {

                LojaIdGlobal = (int)entidade,
                Quantidade = source.Quantidade,
                QuantidadeReservado = null,
                Tipo = Lexos.Hub.Sync.Constantes.Produto.SIMPLES,
                Sku = source.Produto?.CodigoSistema
                    ?? source.Produto?.CodigoInterno
                    ?? source.Produto?.CodigoBarras
                    ?? string.Empty,
                DateVersion = ResolveDateVersion(source)
            };
        }

        private static DateTime ResolveDateVersion(EstoqueResponse source)
        {
            if (!string.IsNullOrWhiteSpace(source.DataAlteracao)
                && DateTime.TryParse(source.DataAlteracao, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var alteracao))
            {
                return alteracao;
            }

            if (!string.IsNullOrWhiteSpace(source.Data)
                && DateTime.TryParse(source.Data, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var data))
            {
                return data;
            }

            return DateTime.UtcNow;
        }
    }
}