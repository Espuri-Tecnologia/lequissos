using System;

namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoPrecoView
    {
        public const int PRECO_VENDA_PADRAO = 1;
        public const int PRECO_PROMOCIONAL_PADRAO = 2;
        public const string CODIGO_PRECO_VENDA_PADRAO = "preco_venda";

        public long ProdutoId { get; set; }
        public long ProdutoIdGlobal { get; set; }
        public string Codigo { get; set; } = CODIGO_PRECO_VENDA_PADRAO;
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public string Tipo { get; set; }
        public string Sku { get; set; }
        public long Quantidade { get; set; }

        public int TipoPrecoId { get; set; }

        /// <summary>
        /// Data atual da ultima versao da entidade, utilizada para controlar o fluxo de atualizacoes, portanto, um item so pode ser alterado caso a nova versao seja superior a atual.
        /// (utilizado para resolver problemas devido as atualizacoes assincronas)
        /// </summary>
        public DateTime DateVersion { get; set; }
    }
}
