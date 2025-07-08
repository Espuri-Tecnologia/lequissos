using System.Collections.Generic;
using System.Linq;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using ProdutoView = Lexos.Hub.Sync.Models.Produto.VarejoOnlineProduto;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers
{
    public static class ProdutoViewMapper
    {
        public static ProdutoView? Map(ProdutoResponse? source)
        {
            return source == null ? null : new ProdutoView
            {
                ProdutoIdGlobal = source.Id,
                Nome = source.Descricao,
                DescricaoResumida = source.DescricaoSimplificada,
                Ean = source.CodigoBarras,
                Peso = source.Peso,
                Comprimento = source.Comprimento,
                Largura = source.Largura,
                Altura = source.Altura
            };
        }

        public static List<ProdutoView> Map(IEnumerable<ProdutoResponse>? source)
        {
            return source?.Select(Map)
                .Where(v => v != null)
                .Cast<ProdutoView>()
                .ToList() ?? new List<ProdutoView>();
        }
    }
}
