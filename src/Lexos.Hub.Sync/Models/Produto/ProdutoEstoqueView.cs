using System;

namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoEstoqueView
    {
        public long ProdutoId { get; set; }
        public long ProdutoIdGlobal { get; set; }
        public long? PedidoId { get; set; }
        public int LojaId { get; set; }
        public long LojaIdGlobal { get; set; }
        public string LocalArm { get; set; }
        public decimal Quantidade { get; set; }
        public decimal? QuantidadeReservado { get; set; }
        public decimal? Custo { get; set; }
        public string Tipo { get; set; }
        public string Sku { get; set; }

        /// <summary>
        /// Data atual da ultima versao da entidade, utilizada para controlar o fluxo de atualizacoes, portanto, um item so pode ser alterado caso a nova versao seja superior a atual.
        /// (utilizado para resolver problemas devido as atualizacoes assincronas)
        /// </summary>
        public DateTime DateVersion { get; set; }

    }
}