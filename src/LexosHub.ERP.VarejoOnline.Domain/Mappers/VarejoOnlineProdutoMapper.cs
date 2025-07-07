using System.Collections.Generic;
using Lexos.Hub.Sync.Models.Produto;

namespace LexosHub.ERP.VarejoOnline.Domain.Mappers
{
    public static class VarejoOnlineProdutoMapper
    {
        public static List<VarejoOnlineProduto> Map(List<VarejoOnlineProdutoDto>? source)
        {
            var result = new List<VarejoOnlineProduto>();
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

                var mapped = new VarejoOnlineProduto
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
