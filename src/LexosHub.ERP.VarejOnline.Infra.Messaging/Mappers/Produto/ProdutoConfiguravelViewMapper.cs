using System;
using System.Collections.Generic;
using System.Linq;
using Lexos.Hub.Sync.Models.Produto;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoConfiguravelViewMapper
    {
        public static ProdutoView? Map(ProdutoResponse produtoBase, List<ProdutoResponse> variacoes)
        {
            if (produtoBase == null)
            {
                return null;
            }

            var produtoView = ProdutoSimplesViewMapper.Map(produtoBase);
            if (produtoView == null)
            {
                return null;
            }

            produtoView.ProdutoTipoId = Lexos.Hub.Sync.Constantes.Produto.CONFIGURAVEL;
            produtoView.Variacoes = variacoes?.Select(MapVariacao).ToList() ?? new List<ProdutoVariacaoView>();

            return produtoView;
        }

        private static ProdutoVariacaoView MapVariacao(ProdutoResponse source)
        {
            var tamanho = source.ValorAtributos?.FirstOrDefault(a => a.Nome.Equals("TAMANHO", StringComparison.OrdinalIgnoreCase))?.Valor;
            var cor = source.ValorAtributos?.FirstOrDefault(a => a.Nome.Equals("COR", StringComparison.OrdinalIgnoreCase))?.Valor;

            return new ProdutoVariacaoView
            {
                ProdutoIdGlobal = source.Id,
                ProdutoId = source.Id,
                Sku = source.CodigoSistema?.Trim(),
                EAN = source.CodigoBarras,
                Tamanho = tamanho,
                Cor = cor,
                Deleted = !source.Ativo,
                OutrasVariacoes = new List<OutraVariacao>(),
                ReferenciasOutrasPlataformas = new List<ProdutoReferenciaView>()
            };
        }
    }
}
