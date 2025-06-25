using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Auth;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;

public interface IVarejoOnlineApiService
{
    Task<Response<TokenResponse?>> ExchangeCodeForTokenAsync(string code);
    Task<string> GetAuthUrl();
    Task<Response<List<EmpresaResponse>>> GetEmpresasAsync(EmpresaRequest request);
    public Task<Response<List<TabelaPrecoListResponse>>> GetPriceTablesAsync(int? inicio = null, int? quantidade = null, string? alteradoApos = null, string? entidades = null);
}
