using System.Collections.Generic;

namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoVariacaoView
    {
        public ProdutoVariacaoView()
        {
            ReferenciasOutrasPlataformas = new List<ProdutoReferenciaView>();
        }

        public long ProdutoIdGlobal { get; set; }
        public string Sku { get; set; }
        public string Tamanho { get; set; }
        public string Cor { get; set; }
        public bool Deleted { get; set; }
        public long ProdutoId { get; set; }
        public List<OutraVariacao> OutrasVariacoes { get; set; }
        public string EAN { get; set; }

        /// <summary>
        /// Utilizado para armazenar o sku original da plataforma, pois em casos onde há importação usando a planilha o "Sku" poderá ser alterado, portanto, precisamos ter o sku original para que possamos fazer o mapeamento correto no processamento do anúncio.
        /// É importante ressaltar para sempre verificar como foi implementando o map do anúncio configurável em uma plataforma, pois esse atributo é essencial para o vinculo correto.
        /// </summary>
        public string SkuOriginalDaPlataforma { get; set; }
        public List<ProdutoReferenciaView> ReferenciasOutrasPlataformas { get; set; }

        /*public void Map(Produto variacao)
        {
            variacao.ProdutoIdGlobal = ProdutoIdGlobal > 0 ? ProdutoIdGlobal : null;
            variacao.Sku = Sku?.Trim();
            variacao.Deleted = Deleted;
        }*/
    }

    public class OutraVariacao
    {
        public string AtributoVaricaoCodigo { get; set; }
        public string AtributoVaricaoDescricao { get; set; }
        public string Valor { get; set; }
    }
}

