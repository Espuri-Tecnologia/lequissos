using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;

namespace LexosHub.ERP.VarejOnline.Domain.Mappers
{
    public static class VarejOnlineProdutoMapper
    {
        public static List<VarejOnlineProduto> Map(List<VarejOnlineProdutoDto>? source)
        {
            var result = new List<VarejOnlineProduto>();
            if (source == null)
            {
                return result;
            }

            foreach (var item in source)
            {
                if (item == null)
                {
                    continue;
                }

                var mapped = new VarejOnlineProduto
                {
                    ProdutoIdGlobal = item.id,
                    Nome = item.descricao,
                    DescricaoResumida = item.descricaoSimplificada,
                    Ean = item.codigoBarras,
                    Peso = item.peso,
                    Comprimento = item.comprimento,
                    Largura = item.largura,
                    Altura = item.altura
                };

                result.Add(mapped);
            }

            return result;
        }
    }
}
