using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Requests.Produto;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Webhook;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Empresa;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses.Entidade;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;

public interface IVarejOnlineApiService
{
    Task<Response<TokenResponse?>> ExchangeCodeForTokenAsync(string code);
    Task<string> GetAuthUrl();
    Task<Response<List<EntidadeResponse>>> GetEntidadesAsync(string token, bool somenteAtivas);
    Task<Response<List<EmpresaResponse>>> GetEmpresasAsync(string token, EmpresaRequest request);
    Task<Response<List<EntidadeResponse>>> GetEntidadesAsync(string token, bool entidadeEcommerce = true);
    Task<Response<List<ProdutoResponse>>> GetProdutosAsync(string token, ProdutoRequest request);
    Task<Response<List<EstoqueResponse>>> GetEstoquesAsync(string token, EstoqueRequest request);
    Task<Response<WebhookOperationResponse>> RegisterWebhookAsync(string token, WebhookRequest payload, CancellationToken cancellationToken = default);
    Task<Response<List<TabelaPrecoListResponse>>> GetPriceTablesAsync(string token, TabelaPrecoRequest request);
    Task<Response<PedidoResponse>> PostPedidoAsync(string token, PedidoRequest request);
    Task<Response<PedidoResponse>> AlterarStatusPedidoAsync(string token, long pedidoNumero, string novoStatus);
}
