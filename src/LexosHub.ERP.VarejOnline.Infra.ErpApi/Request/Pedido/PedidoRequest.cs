using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
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

        [JsonProperty("cnpj")]
        public string? Cnpj { get; set; }

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

        [JsonProperty("servicos")]
        public List<ServicoPedido>? Servicos { get; set; }

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

        /// <summary>URL de impresso da etiqueta (opcional).</summary>
        [JsonProperty("urlEtiqueta")]
        public string? UrlEtiqueta { get; set; }
    }
}
