using System.Collections.Generic;

namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses
{
    public class ProdutoResponse
    {
        public long Id { get; set; }
        public bool Ativo { get; set; }
        public string? DataAlteracao { get; set; }
        public string? DataCriacao { get; set; }
        public List<string>? CnpjFornecedores { get; set; }
        public List<FornecedorResponse>? Fornecedores { get; set; }
        public long? IdFabricante { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? DescricaoSimplificada { get; set; }
        public string? Especificacao { get; set; }
        public string CodigoSku { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Comprimento { get; set; }
        public decimal? Largura { get; set; }
        public string? CodigoBarras { get; set; }
        public List<string>? CodigosBarraAdicionais { get; set; }
        public string? CodigoInterno { get; set; }
        public string? CodigoSistema { get; set; }
        public long? ProdutoEquivalente { get; set; }
        public long? ProdutoBaseSeminovo { get; set; }
        public string? Tags { get; set; }
        public string? Unidade { get; set; }
        public List<UnidadeProporcaoResponse>? UnidadesProporcao { get; set; }
        public string? Classificacao { get; set; }
        public long? Origem { get; set; }
        public string? Fci { get; set; }
        public string? CodigoCest { get; set; }
        public string? CodigoNcm { get; set; }
        public string? MetodoControle { get; set; }
        public bool? PermiteVenda { get; set; }
        public decimal? CustoReferencial { get; set; }
        public List<CustoReferencialResponse>? ListCustoReferencial { get; set; }
        public decimal? Preco { get; set; }
        public decimal? DescontoMaximo { get; set; }
        public decimal? Comissao { get; set; }
        public decimal? MargemLucro { get; set; }
        public decimal? EstoqueMaximo { get; set; }
        public decimal? EstoqueMinimo { get; set; }
        public List<DadosPorEntidadeResponse>? DadosPorEntidade { get; set; }
        public List<ValorAtributoResponse>? ValorAtributos { get; set; }
        public List<CategoriaResponse>? Categorias { get; set; }
        public ProdutoBaseResponse? ProdutoBase { get; set; }
        public List<string>? UrlsFotosProduto { get; set; }
        public bool? DisponivelEcommerce { get; set; }
        public bool? DisponivelMarketplace { get; set; }
        public decimal? CustoUnitarioRoyalties { get; set; }
        public bool? PrecoVariavel { get; set; }
        public bool? AmostraGratis { get; set; }
        public bool? DefinirPrecoVendaPelaFichaTecnica { get; set; }
        public List<ComponenteResponse>? Componentes { get; set; }
        public List<DescontoProgressivoResponse>? DescontoProgressivo { get; set; }
        public List<AtributoProdutoResponse>? AtributosProduto { get; set; }
        public List<PrecoPorTabelaResponse>? PrecosPorTabelas { get; set; }
    }
}
