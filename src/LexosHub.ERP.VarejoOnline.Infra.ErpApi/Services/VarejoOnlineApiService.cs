using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Auth;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;


namespace LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Services
{
    public class VarejoOnlineApiService : IVarejoOnlineApiService
    {
        private readonly VarejoOnlineApiSettings _erpApiSettings;
        private RestClient _client;
        private string _oAuthGetTokenUrl;
        private string _oAuthRedirectUrl;
        private string _clientId;
        private string _clientSecret;
        private string _oAuthUrl;

        public VarejoOnlineApiService(IOptions<VarejoOnlineApiSettings> erpApiSettings)
        {
            _erpApiSettings = erpApiSettings.Value;

            if (string.IsNullOrEmpty(_erpApiSettings?.BaseUrl))
            {
                throw new ArgumentNullException(nameof(_erpApiSettings.BaseUrl));
            }

            _client = new RestClient(_erpApiSettings.BaseUrl);
            _oAuthGetTokenUrl = _erpApiSettings?.OAuthGetTokenUrl ?? string.Empty;
            _oAuthRedirectUrl = _erpApiSettings?.OAuthRedirectUrl ?? string.Empty;
            _clientId = _erpApiSettings?.ClientId ?? string.Empty;
            _clientSecret = _erpApiSettings?.ClientSecret ?? string.Empty;
            _oAuthUrl = _erpApiSettings?.OAuthUrl ?? string.Empty;
        }

        #region Auth
        public async Task<Response<TokenResponse?>> ExchangeCodeForTokenAsync(string code)
        {
            _client = new RestClient(_erpApiSettings.OAuthGetTokenUrl);

            var request = new RestRequest("/apps/oauth/token", Method.Post)
                .AddHeader("Content-Type", "application/json");

            var tokenRequest = new TokenRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                RedirectUri = _oAuthRedirectUrl,
                Code = code
            };

            var json = JsonConvert.SerializeObject(tokenRequest, Formatting.Indented);

            request
              .AddHeader("Content-Type", "application/json")
              .AddJsonBody(json);

            return await ExecuteAsync<TokenResponse?>(request);
        }
        public Task<string> GetAuthUrl()
        {
            return Task.FromResult<string>($"{_erpApiSettings.BaseUrl}{_erpApiSettings.OAuthUrl}client_id={_clientId}&redirect_uri={_oAuthRedirectUrl}");
        }
        #endregion

        #region Empresas
        public async Task<Response<List<EmpresaResponse>>> GetEmpresasAsync(EmpresaRequest request)
        {
            var restRequest = new RestRequest("empresas", Method.Get);

            if (request.Inicio.HasValue)
                restRequest.AddQueryParameter("inicio", request.Inicio.Value.ToString());

            if (request.Quantidade.HasValue)
                restRequest.AddQueryParameter("quantidade", request.Quantidade.Value.ToString());

            if (!string.IsNullOrWhiteSpace(request.AlteradoApos))
                restRequest.AddQueryParameter("alteradoApos", request.AlteradoApos);

            if (!string.IsNullOrWhiteSpace(request.Status))
                restRequest.AddQueryParameter("status", request.Status);

            if (!string.IsNullOrWhiteSpace(request.CampoCustomizadoNome))
                restRequest.AddQueryParameter("campoCustomizadoNome", request.CampoCustomizadoNome);

            if (!string.IsNullOrWhiteSpace(request.CampoCustomizadoValor))
                restRequest.AddQueryParameter("campoCustomizadoValor", request.CampoCustomizadoValor);

            if (!string.IsNullOrWhiteSpace(request.Cnpj))
                restRequest.AddQueryParameter("cnpj", request.Cnpj);

            return await ExecuteAsync<List<EmpresaResponse>>(restRequest);
        }

        #endregion

        #region Prices

        public async Task<Response<List<TabelaPrecoListResponse>>> GetPriceTablesAsync(int? inicio = null, int? quantidade = null, string? alteradoApos = null, string? entidades = null)
        {
            var request = new RestRequest("tabelas-preco", Method.Get);

            if (inicio.HasValue)
                request.AddQueryParameter("inicio", inicio.Value.ToString());

            if (quantidade.HasValue)
                request.AddQueryParameter("quantidade", quantidade.Value.ToString());

            if (!string.IsNullOrWhiteSpace(alteradoApos))
                request.AddQueryParameter("alteradoApos", alteradoApos);

            if (!string.IsNullOrWhiteSpace(entidades))
                request.AddQueryParameter("entidades", entidades);

            return await ExecuteAsync<List<TabelaPrecoListResponse>>(request);
        }
        #endregion

        #region Utils
        private async Task<Response<T>> ExecuteAsync<T>(RestRequest request)
        {
            try
            {
                request.AddHeader("Content-Type", "application/json");

                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new Response<T> { Error = GetErrorMessageResponse(response) };

                return new Response<T>(JsonConvert.DeserializeObject<T>(response.Content!)!);
            }
            catch (Exception ex)
            {
                return new Response<T> { Error = new ErrorResult($"{ex.Message}") };
            }
        }

        private ErrorResult GetErrorMessageResponse(RestResponse response)
        {
            try
            {
                var badRequestResponse = JsonConvert.DeserializeObject<BadRequestResponse>(response.Content);

                if (badRequestResponse != null)
                {
                    string errorMesssage = $"{badRequestResponse.code} - {badRequestResponse.message} - {badRequestResponse.detailedMessage}";

                    if (badRequestResponse.details.Any())
                    {
                        errorMesssage += "\nDetalhes:\n";
                        badRequestResponse.details.ForEach(error => { errorMesssage += $"{error.code} - {error.message} - {error.detailedMessage} \n"; });
                    }

                    return new ErrorResult(errorMesssage);
                }

                return new ErrorResult($"{response.StatusDescription} - {response.ResponseUri} - {response.Content}");
            }
            catch
            {
                return new ErrorResult($"{response.StatusDescription} - {response.ResponseUri} - {response.Content}");
            }
        }

        #endregion
    }
}
