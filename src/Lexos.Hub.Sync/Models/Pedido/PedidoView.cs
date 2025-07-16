using System;
using System.Collections.Generic;

namespace Lexos.Hub.Sync.Models.Pedido
{
    public class PedidoView
    {
        public long PedidoId { get; set; }
        public string PedidoERPId { get; set; }
        public string PedidoERPCodigo { get; set; }
        public string Codigo { get; set; }
        public DateTime Data { get; set; }

        public string Status { get; set; }
        public bool PagamentoAprovado { get; set; }
        public bool PedidoCancelado { get; set; }

        public bool PedidoJaSaiuParaEntrega { get; set; }

        public decimal? Desconto { get; set; }
        public decimal? Acrescimo { get; set; }
        public decimal? Frete { get; set; }
        public decimal FreteCusto { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Total { get; set; }
        public string TipoFrete { get; set; }
        public string EntregasStatus { get; set; }
        public string ClienteCpfcnpj { get; set; }
        public string ClienteNome { get; set; }
        public string ClienteNomeFantasia { get; set; }
        public DateTime? ClienteDataNascimento { get; set; }
        public string ClienteSexo { get; set; }
        public string ClienteRg { get; set; }
        public string Observacao { get; set; }
        public string CodigoRastreio { get; set; }
        public string CodigoPlataforma { get; set; }
        public string Plataforma { get; set; }
        public string Canal { get; set; }
        public short CanalId { get; set; }
        public long LojaId { get; set; }
        public string NomeExternoLoja { get; set; }
        public long LojaFaturamentoId { get; set; }
        public long? PedidoStatusERPId { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public long? CodigoLojaErpExterno { get; set; }

        public PedidoCampanhaView PedidoCampanhaView { get; set; }

        public List<PedidoClienteContatoView> Contatos { get; set; }
        public List<PedidoClienteEnderecoView> Enderecos { get; set; }
        public List<PedidoComposicaoPagamentoView> ComposicaoPagamento { get; set; }
        public List<PedidoItemView> Itens { get; set; }
        public List<PedidoNFeView> NFes { get; set; }

        public string IntegracaoDescricao { get; set; }
        public string IntegracaoId { get; set; }
        public bool IsFulfillment { get; set; }
        public string InscricaoEstadual { get; set; }
        public string TipoLogistica { get; set; }
        public string TransportadoraNome { get; set; }

    }
}
