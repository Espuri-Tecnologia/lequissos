namespace Lexos.Hub.Sync.Enums
{
    public enum TipoProcessoAtualizacao
    {
        Produto = 1,
        Estoque = 2,
        Preco = 3,
        Pedido = 4,
        CadastrarPedidoErp = 5,
        SucessoRenovacaoToken = 6,
        ErroRenovacaoToken = 7,
        AtualizarEstoqueViaPlanilha = 8,
        ExcluirDesvincular = 9,
        PublicarAnuncioPorProduto = 10,
        PublicarAnuncioEmMassa = 11,
        GerarCargaAnuncio = 12,
        CriarCargaProduto = 13,
        MapearItensPedido = 14,
        AgendarCriacaoDeCargasDeEstoque = 15,
        ReprocessarPedidos = 16,
        PublicarAnuncioEmMassaIntegracao = 17,
        PublicarAnuncioEmMassaProdutos = 18,
        CriarCargasEstoques = 19,
        AdicionarListaSeparacao = 20,
        AtualizarProdutosEmMassa = 21,
        NotaFiscal = 22
    }
}

