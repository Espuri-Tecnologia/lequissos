using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Auth;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejoOnline.Infra.ErpApi.Responses.Webhook;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Request;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

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
        private string _webHookEnpoint;

        // JsonSerializerOptions global para System.Text.Json
        private static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public VarejoOnlineApiService(IOptions<VarejoOnlineApiSettings> erpApiSettings)
        {
            _erpApiSettings = erpApiSettings.Value ?? throw new ArgumentNullException(nameof(erpApiSettings.Value));

            if (string.IsNullOrEmpty(_erpApiSettings.BaseUrl))
                throw new ArgumentNullException(nameof(_erpApiSettings.BaseUrl));

            _client = new RestClient(_erpApiSettings.BaseUrl);
            _oAuthGetTokenUrl = _erpApiSettings.OAuthGetTokenUrl ?? string.Empty;
            _oAuthRedirectUrl = _erpApiSettings.OAuthRedirectUrl ?? string.Empty;
            _clientId = _erpApiSettings.ClientId ?? string.Empty;
            _clientSecret = _erpApiSettings.ClientSecret ?? string.Empty;
            _oAuthUrl = _erpApiSettings.OAuthUrl ?? string.Empty;
            _oAuthUrl = _erpApiSettings.OAuthUrl ?? string.Empty;
            _webHookEnpoint = _erpApiSettings.WebhookEndpoint ?? string.Empty;
        }

        #region Auth
        public async Task<Response<TokenResponse?>> ExchangeCodeForTokenAsync(string code)
        {
            _client = new RestClient(_oAuthGetTokenUrl);

            var request = new RestRequest("/apps/oauth/token", Method.Post)
                .AddHeader("Content-Type", "application/json");

            var tokenRequest = new TokenRequest
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                RedirectUri = _oAuthRedirectUrl,
                Code = code
            };

            request.AddJsonBody(tokenRequest);

            return await ExecuteAsync<TokenResponse?>(request);
        }

        public Task<string> GetAuthUrl()
        {
            return Task.FromResult($"{_erpApiSettings.BaseUrl}{_oAuthUrl}client_id={_clientId}&redirect_uri={_oAuthRedirectUrl}");
        }
        #endregion

        #region Empresas
        public async Task<Response<List<EmpresaResponse>>> GetEmpresasAsync(string token, EmpresaRequest request)
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

            return await ExecuteAsync<List<EmpresaResponse>>(restRequest, token);
        }
        #endregion

        #region Prices
        public async Task<Response<List<TabelaPrecoListResponse>>> GetPriceTablesAsync(string token, int inicio = 0, int quantidade = 10, string? alteradoApos = null, string? entidades = null)
        {
            var request = new RestRequest("tabelas-preco", Method.Get);

            request.AddQueryParameter("inicio", inicio.ToString());

            request.AddQueryParameter("quantidade", quantidade.ToString());

            if (!string.IsNullOrWhiteSpace(alteradoApos))
                request.AddQueryParameter("alteradoApos", alteradoApos);

            if (!string.IsNullOrWhiteSpace(entidades))
                request.AddQueryParameter("entidades", entidades);

            return await ExecuteAsync<List<TabelaPrecoListResponse>>(request, token);
        }
        #endregion

        #region Produtos
        public async Task<Response<List<ProdutoResponse>>> GetProdutosAsync(string token, ProdutoRequest request)
        {
            var resource = request.Id.HasValue ? $"apps/api/produtos/{request.Id.Value}" : "apps/api/produtos";
            var restRequest = new RestRequest(resource, Method.Get);

            if (request.Inicio.HasValue)
                restRequest.AddQueryParameter("inicio", request.Inicio.Value.ToString());

            if (request.Quantidade.HasValue && request.Quantidade.Value > 0)
                restRequest.AddQueryParameter("quantidade", request.Quantidade.Value.ToString());

            if (!string.IsNullOrWhiteSpace(request.AlteradoApos))
                restRequest.AddQueryParameter("alteradoApos", request.AlteradoApos);

            if (!string.IsNullOrWhiteSpace(request.Categoria))
                restRequest.AddQueryParameter("categoria", request.Categoria);

            if (request.ProdutoBase.HasValue)
                request.ProdutoBase.Value.ToString();

            if (!string.IsNullOrWhiteSpace(request.Descricao))
                restRequest.AddQueryParameter("descricao", request.Descricao);

            if (!string.IsNullOrWhiteSpace(request.CodigoBarras))
                restRequest.AddQueryParameter("codigoBarras", request.CodigoBarras);

            if (!string.IsNullOrWhiteSpace(request.CodigoInterno))
                restRequest.AddQueryParameter("codigoInterno", request.CodigoInterno);

            if (!string.IsNullOrWhiteSpace(request.CodigoSistema))
                restRequest.AddQueryParameter("codigoSistema", request.CodigoSistema);

            if (request.SomenteAtivos.HasValue)
                restRequest.AddQueryParameter("somenteAtivos", request.SomenteAtivos.Value.ToString().ToLower());

            if (request.SomenteComFotos.HasValue)
                restRequest.AddQueryParameter("somenteComFotos", request.SomenteComFotos.Value.ToString().ToLower());

            if (request.SomenteEcommerce.HasValue)
                restRequest.AddQueryParameter("somenteEcommerce", request.SomenteEcommerce.Value.ToString().ToLower());

            if (request.SomenteMarketplace.HasValue)
                restRequest.AddQueryParameter("somenteMarketplace", request.SomenteMarketplace.Value.ToString().ToLower());

            if (request.AmostraGratis.HasValue)
                restRequest.AddQueryParameter("amostraGratis", request.AmostraGratis.Value.ToString().ToLower());

            if (!string.IsNullOrWhiteSpace(request.AlteracaoDesde))
                restRequest.AddQueryParameter("alteracaoDesde", request.AlteracaoDesde);

            if (!string.IsNullOrWhiteSpace(request.AlteracaoAte))
                restRequest.AddQueryParameter("alteracaoAte", request.AlteracaoAte);

            if (!string.IsNullOrWhiteSpace(request.CriacaoDesde))
                restRequest.AddQueryParameter("criacaoDesde", request.CriacaoDesde);

            if (!string.IsNullOrWhiteSpace(request.CriacaoAte))
                restRequest.AddQueryParameter("criacaoAte", request.CriacaoAte);

            if (!string.IsNullOrWhiteSpace(request.IdsProdutos))
                restRequest.AddQueryParameter("idsProdutos", request.IdsProdutos);

            if (!string.IsNullOrWhiteSpace(request.IdsTabelasPrecos))
                restRequest.AddQueryParameter("idsTabelasPrecos", request.IdsTabelasPrecos);

            return await ExecuteAsync<List<ProdutoResponse>>(restRequest, token);
        }
        #endregion

        #region Estoques
        public async Task<Response<List<EstoqueResponse>>> GetEstoquesAsync(string token, EstoqueRequest request)
        {
            var restRequest = new RestRequest("saldos-mercadorias", Method.Get);

            if (!string.IsNullOrWhiteSpace(request.Produtos))
                restRequest.AddQueryParameter("produtos", request.Produtos);

            if (!string.IsNullOrWhiteSpace(request.Entidades))
                restRequest.AddQueryParameter("entidades", request.Entidades);

            if (request.Inicio.HasValue)
                restRequest.AddQueryParameter("inicio", request.Inicio.Value.ToString());

            if (request.Quantidade.HasValue)
                restRequest.AddQueryParameter("quantidade", request.Quantidade.Value.ToString());

            if (!string.IsNullOrWhiteSpace(request.AlteradoApos))
                restRequest.AddQueryParameter("alteradoApos", request.AlteradoApos);

            if (!string.IsNullOrWhiteSpace(request.Data))
                restRequest.AddQueryParameter("data", request.Data);

            if (request.SomenteEcommerce.HasValue)
                restRequest.AddQueryParameter("somenteEcommerce", request.SomenteEcommerce.Value.ToString().ToLower());

            if (request.SomenteMarketplace.HasValue)
                restRequest.AddQueryParameter("somenteMarketplace", request.SomenteMarketplace.Value.ToString().ToLower());

            return await ExecuteAsync<List<EstoqueResponse>>(restRequest, token);
        }
        #endregion

        #region WebhookRegister
        public async Task<Response<WebhookResponse>> RegisterWebhookAsync(string token,
    WebhookRequest payload,
    CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(_webHookEnpoint, Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(payload);

            return await ExecuteAsync<WebhookResponse>(request, token);
        }
        #endregion

        #region Utils
        private async Task<Response<T>> ExecuteAsync<T>(RestRequest request, string? token = null)
        {
            try
            {
                request.AddHeader("Content-Type", "application/json");

                if (!string.IsNullOrWhiteSpace(token))
                    request.AddQueryParameter("token", token);

                var response = await _client.ExecuteAsync(request);

                if (!response.IsSuccessStatusCode)
                    return new Response<T> { Error = GetErrorMessageResponse(response), StatusCode = response.StatusCode };

                var content = response.Content!;

                var type = typeof(T);
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
                {
                    if (!content.TrimStart().StartsWith("["))
                    {
                        var itemType = type.GetGenericArguments()[0];
                        var singleItem = JsonSerializer.Deserialize(content, itemType, DefaultJsonOptions);

                        var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                        if (singleItem != null)
                            list.Add(singleItem);

                        return new Response<T>((T)list) { StatusCode = response.StatusCode };
                    }
                }
                var result = JsonSerializer.Deserialize<T>(content, DefaultJsonOptions);
                return new Response<T>(result!) { StatusCode = response.StatusCode };
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
                var badRequestResponse = JsonSerializer.Deserialize<BadRequestResponse>(response.Content!, DefaultJsonOptions);

                if (badRequestResponse != null)
                {
                    string errorMesssage = $"{badRequestResponse.code} - {badRequestResponse.mensagem} - {badRequestResponse.detalhes}";

                    if (badRequestResponse.details != null && badRequestResponse.details.Any())
                    {
                        errorMesssage += "\nDetalhes:\n";
                        badRequestResponse.details.ForEach(error =>
                        {
                            errorMesssage += $"{error.code} - {error.mensagem} - {error.detalhes} \n";
                        });
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
