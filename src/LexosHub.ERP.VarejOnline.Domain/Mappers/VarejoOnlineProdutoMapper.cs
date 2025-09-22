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
                    Nome = item.descricao?.Trim(),
                    DescricaoResumida = item.descricaoSimplificada?.Trim(),
                    Ean = item.codigoBarras?.Trim(),
                    Peso = item.peso ?? 0,
                    Comprimento = item.comprimento ?? 0,
                    Largura = item.largura ?? 0,
                    Altura = item.altura ?? 0
                };

                result.Add(mapped);
            }

            return result;
        }
    }
}
