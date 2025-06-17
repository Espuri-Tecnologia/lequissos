using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Auth;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;

public interface IVarejoOnlineApiService
{
    Task<Response<TokenResponse?>> ExchangeCodeForTokenAsync(string code);
    Task<string> GetAuthUrl();
    public Task<Response<List<PriceTableListResponse>>> GetPriceTablesAsync(int? inicio = null, int? quantidade = null, string? alteradoApos = null, string? entidades = null);
}
