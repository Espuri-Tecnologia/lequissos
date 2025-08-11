using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request
{
    /// <summary>
    /// Modelo alinhado ao POST /pedidos do Varejo Online.
    /// </summary>
    public class PedidoRequest
    {
        [JsonProperty("numeroPedidoCliente")]
        public string NumeroPedidoCliente { get; set; } = string.Empty;

        [JsonProperty("idOrcamento")]
        public long? IdOrcamento { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("data")]
        public string? Data { get; set; }

        /// <summary>hh:mm:ss</summary>
        [JsonProperty("horario")]
        public string? Horario { get; set; }

        [JsonProperty("entidade")]
        public EntidadeRef? Entidade { get; set; }

        [JsonProperty("terceiro")]
        public TerceiroRef? Terceiro { get; set; }

        /// <summary>Ignorado se pagamentos forem informados.</summary>
        [JsonProperty("plano")]
        public PlanoRef? Plano { get; set; }

        [JsonProperty("representante")]
        public RepresentanteRef? Representante { get; set; }

        [JsonProperty("valorDesconto")]
        public decimal? ValorDesconto { get; set; }

        [JsonProperty("descontoDetalhes")]
        public List<DetalheDesconto>? DescontoDetalhes { get; set; }

        [JsonProperty("observacao")]
        public string? Observacao { get; set; }

        [JsonProperty("valorFrete")]
        public decimal? ValorFrete { get; set; }

        [JsonProperty("valorOutros")]
        public decimal? ValorOutros { get; set; }

        [JsonProperty("valorSeguro")]
        public decimal? ValorSeguro { get; set; }

        [JsonProperty("vendaConsumidorFinal")]
        public bool? VendaConsumidorFinal { get; set; }

        [JsonProperty("itens")]
        public List<ItemPedido>? Itens { get; set; }

        [JsonProperty("emitirNotaFiscal")]
        public bool? EmitirNotaFiscal { get; set; }

        [JsonProperty("emitirNotaFiscalPresente")]
        public bool? EmitirNotaFiscalPresente { get; set; }

        [JsonProperty("enviarEmailNota")]
        public bool? EnviarEmailNota { get; set; }

        /// <summary>ECOMMERCE, MARKETPLACE, LOJA_FISICA</summary>
        [JsonProperty("origem")]
        public string? Origem { get; set; }

        /// <summary>NORMAL, SHIP_FROM_STORE, CLICK_COLLECT</summary>
        [JsonProperty("tipo")]
        public string? Tipo { get; set; }

        [JsonProperty("intermediador")]
        public TerceiroRef? Intermediador { get; set; }

        [JsonProperty("transporte")]
        public Transporte? Transporte { get; set; }

        [JsonProperty("enderecoEntrega")]
        public EnderecoEntrega? EnderecoEntrega { get; set; }

        [JsonProperty("pagamento")]
        public Pagamento? Pagamento { get; set; }

        /// <summary>URL de impressão da etiqueta (opcional).</summary>
        [JsonProperty("urlEtiqueta")]
        public string? UrlEtiqueta { get; set; }
    }

    #region Cabeçalho: Entidade / Terceiro / Representante / Plano
    public class EntidadeRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("documento")]
        public string? Documento { get; set; }
    }

    public class TerceiroRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("documento")]
        public string? Documento { get; set; }
    }

    public class RepresentanteRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("nome")]
        public string? Nome { get; set; }
    }

    public class PlanoRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("descricao")]
        public string? Descricao { get; set; }
    }
    #endregion

    #region Itens
    public class ItemPedido
    {
        [JsonProperty("produto")]
        public ProdutoRef? Produto { get; set; }

        [JsonProperty("quantidade")]
        public decimal? Quantidade { get; set; }

        [JsonProperty("valorDesconto")]
        public decimal? ValorDesconto { get; set; }

        [JsonProperty("valesPresentes")]
        public List<ValePresente>? ValesPresentes { get; set; }

        [JsonProperty("descontoDetalhes")]
        public List<DetalheDesconto>? DescontoDetalhes { get; set; }

        /// <summary>Valor unitário antes do desconto.</summary>
        [JsonProperty("valorUnitario")]
        public decimal? ValorUnitario { get; set; }

        /// <summary>Id da operação do item.</summary>
        [JsonProperty("operacao")]
        public long? Operacao { get; set; }

        /// <summary>CFOP do item (quando aplicável).</summary>
        [JsonProperty("cfop")]
        public string? Cfop { get; set; }
    }

    public class ProdutoRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("codigoSistema")]
        public string? CodigoSistema { get; set; }

        [JsonProperty("codigoInterno")]
        public string? CodigoInterno { get; set; }

        [JsonProperty("codigoBarras")]
        public string? CodigoBarras { get; set; }
    }

    public class ValePresente
    {
        [JsonProperty("numeroValePresente")]
        public string? NumeroValePresente { get; set; }

        [JsonProperty("emailValePresente")]
        public string? EmailValePresente { get; set; }

        /// <summary>FISICO, DIGITAL, AMBOS</summary>
        [JsonProperty("formatoValePresente")]
        public string? FormatoValePresente { get; set; }

        [JsonProperty("nomePresenteado")]
        public string? NomePresenteado { get; set; }

        /// <summary>PROPRIO, PRESENTE</summary>
        [JsonProperty("usoValePresente")]
        public string? UsoValePresente { get; set; }
    }

    public class DetalheDesconto
    {
        [JsonProperty("observacao")]
        public string? Observacao { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        /// <summary>Origem do desconto (texto conforme wiki).</summary>
        [JsonProperty("origemDesconto")]
        public string? OrigemDesconto { get; set; }
    }
    #endregion

    #region Transporte & Endereço
    public class Transporte
    {
        /// <summary>Modalidade do transporte (texto conforme wiki).</summary>
        [JsonProperty("modalidade")]
        public string? Modalidade { get; set; }

        [JsonProperty("transportador")]
        public TerceiroRef? Transportador { get; set; }

        [JsonProperty("codigoANTT")]
        public string? CodigoANTT { get; set; }

        [JsonProperty("placaVeiculo")]
        public string? PlacaVeiculo { get; set; }

        [JsonProperty("estadoVeiculo")]
        public string? EstadoVeiculo { get; set; }

        [JsonProperty("quantidade")]
        public long? Quantidade { get; set; }

        [JsonProperty("especie")]
        public string? Especie { get; set; }

        [JsonProperty("marca")]
        public string? Marca { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        [JsonProperty("pesoBruto")]
        public decimal? PesoBruto { get; set; }

        [JsonProperty("pesoLiquido")]
        public decimal? PesoLiquido { get; set; }
    }

    public class EnderecoEntrega
    {
        [JsonProperty("logradouro")]
        public string? Logradouro { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        [JsonProperty("bairro")]
        public string? Bairro { get; set; }

        [JsonProperty("complemento")]
        public string? Complemento { get; set; }

        [JsonProperty("cep")]
        public string? Cep { get; set; }

        [JsonProperty("cidade")]
        public string? Cidade { get; set; }

        [JsonProperty("uf")]
        public string? Uf { get; set; }

        [JsonProperty("receptorEntrega")]
        public string? ReceptorEntrega { get; set; }

        [JsonProperty("receptorEntregaDocumento")]
        public string? ReceptorEntregaDocumento { get; set; }
    }
    #endregion

    #region Pagamento
    public class Pagamento
    {
        [JsonProperty("valorDinheiro")]
        public decimal? ValorDinheiro { get; set; }

        [JsonProperty("cartoes")]
        public List<CartaoPagamento>? Cartoes { get; set; }

        [JsonProperty("cheques")]
        public List<ChequePagamento>? Cheques { get; set; }

        [JsonProperty("adiantamentos")]
        public List<AdiantamentoPagamento>? Adiantamentos { get; set; }

        [JsonProperty("crediario")]
        public CrediarioPagamento? Crediario { get; set; }

        [JsonProperty("boletos")]
        public List<BoletoPagamento>? Boletos { get; set; }

        [JsonProperty("vouchers")]
        public List<VoucherPagamento>? Vouchers { get; set; }

        [JsonProperty("pixes")]
        public List<PixPagamento>? Pixes { get; set; }
    }

    public class CartaoPagamento
    {
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("autorizacao")]
        public string? Autorizacao { get; set; }

        [JsonProperty("quantidadeParcelas")]
        public int? QuantidadeParcelas { get; set; }

        [JsonProperty("nsu")]
        public string? Nsu { get; set; }

        /// <summary>Id da negociação, se utilizada.</summary>
        [JsonProperty("negociacao")]
        public long? Negociacao { get; set; }

        /// <summary>Obrigatório se não informado negociacao.</summary>
        [JsonProperty("operadoraNome")]
        public string? OperadoraNome { get; set; }

        /// <summary>Obrigatório se não informado negociacao.</summary>
        [JsonProperty("bandeiraNome")]
        public string? BandeiraNome { get; set; }

        /// <summary>CREDITO ou DEBITO (quando não houver negociacao).</summary>
        [JsonProperty("tipo")]
        public string? Tipo { get; set; }

        /// <summary>SEM_PARCELAMENTO, PARCELADO_VENDEDOR, PARCELADO_OPERADORA.</summary>
        [JsonProperty("parcelamento")]
        public string? Parcelamento { get; set; }
    }

    public class ChequePagamento
    {
        [JsonProperty("titular")]
        public long? Titular { get; set; }

        [JsonProperty("banco")]
        public long? Banco { get; set; }

        [JsonProperty("agencia")]
        public string? Agencia { get; set; }

        [JsonProperty("conta")]
        public string? Conta { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataEmissao")]
        public string? DataEmissao { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataVencimento")]
        public string? DataVencimento { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }

    public class AdiantamentoPagamento
    {
        /// <summary>Id do adiantamento recebido.</summary>
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }

    public class CrediarioPagamento
    {
        /// <summary>Id do plano de pagamento.</summary>
        [JsonProperty("plano")]
        public long? Plano { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("valorAcrescimo")]
        public decimal? ValorAcrescimo { get; set; }

        [JsonProperty("parcelas")]
        public List<ParcelaCrediario>? Parcelas { get; set; }
    }

    public class ParcelaCrediario
    {
        [JsonProperty("numero")]
        public long? Numero { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("vencimento")]
        public string? Vencimento { get; set; }
    }

    public class BoletoPagamento
    {
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("identificacao")]
        public string? Identificacao { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataVencimento")]
        public string? DataVencimento { get; set; }

        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataPagamento")]
        public string? DataPagamento { get; set; }

        [JsonProperty("codigoConta")]
        public string? CodigoConta { get; set; }
    }

    public class VoucherPagamento
    {
        [JsonProperty("voucher")]
        public VoucherRef? Voucher { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }

    public class VoucherRef
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
    }

    public class PixPagamento
    {
        /// <summary>dd-MM-yyyy</summary>
        [JsonProperty("dataPagamento")]
        public string? DataPagamento { get; set; }

        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        [JsonProperty("nsu")]
        public string? Nsu { get; set; }

        [JsonProperty("autorizacao")]
        public string? Autorizacao { get; set; }
    }
    #endregion
}
