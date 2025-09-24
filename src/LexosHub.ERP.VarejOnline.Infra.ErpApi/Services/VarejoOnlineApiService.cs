using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Requests.Produto;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Auth;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Prices;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Responses.Webhook;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses.Clientes;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;


namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Services
{
    public class VarejOnlineApiService : IVarejOnlineApiService
    {
        private readonly VarejOnlineApiSettings _erpApiSettings;
        private RestClient _client;
        private string _oAuthGetTokenUrl;
        private string _oAuthRedirectUrl;
        private string _clientId;
        private string _clientSecret;
        private string _oAuthUrl;
        private string _webHookEnpoint;

        private static readonly JsonSerializerSettings DefaultJsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };

        public VarejOnlineApiService(IOptions<VarejOnlineApiSettings> erpApiSettings)
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
        public async Task<Response<TokenResponse>> ExchangeCodeForTokenAsync(string code)
        {
            _client = new RestClient(_oAuthGetTokenUrl);
            var tokenRequest = new
            {
                grant_type = "authorization_code",
                client_id = _clientId,
                client_secret = _clientSecret,
                redirect_uri = _oAuthRedirectUrl,
                code = code
            };

            var request = new RestRequest("apps/oauth/token", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddHeader("Cookie", "LB=erp01")
                .AddJsonBody(tokenRequest);

            return await ExecuteAsync<TokenResponse>(request);
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

        #region Entidades
        public async Task<Response<List<EntidadeResponse>>> GetEntidadesAsync(string token)
        {
            var resource = "apps/api/entidades";
            var restRequest = new RestRequest(resource, Method.Get);
            return await ExecuteAsync<List<EntidadeResponse>>(restRequest, token);
        }
        #endregion

        #region Terceiros
        public async Task<Response<List<TerceiroResponse>>> GetTerceirosAsync(string token, TerceiroQueryRequest request)
        {
            var restRequest = new RestRequest("apps/api/terceiros", Method.Get);
            restRequest.AddQueryParameter("documento", request.Documento);
            return await ExecuteAsync<List<TerceiroResponse>>(restRequest, token);
        }

        public async Task<Response<OperationResponse>> CreateTerceiroAsync(string token, TerceiroRequest request)
        {
            var restRequest = new RestRequest("apps/api/terceiros", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(request);

            return await ExecuteAsync<OperationResponse>(restRequest, token);
        }
        #endregion

        #region Prices
        public async Task<Response<List<TabelaPrecoListResponse>>> GetPriceTablesAsync(string token, TabelaPrecoRequest request)
        {
            var resource = request.Id.HasValue ? $"apps/api/tabelas-preco/{request.Id.Value}" : "apps/api/tabelas-preco";
            var restRequest = new RestRequest(resource, Method.Get);

            restRequest.AddQueryParameter("inicio", request.Inicio.ToString());

            restRequest.AddQueryParameter("quantidade", request.Quantidade.ToString());

            return await ExecuteAsync<List<TabelaPrecoListResponse>>(restRequest, token);
        }
        #endregion

        #region Produtos
        public async Task<Response<List<ProdutoResponse>>> GetProdutosAsync(string token, ProdutoRequest request)
        {
            //var resource = request.Id.HasValue ? $"apps/api/produtos/{request.Id.Value}" : "apps/api/produtos";
            var resource = "apps/api/produtos";
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
                restRequest.AddQueryParameter("produtoBase", request.ProdutoBase.Value.ToString());

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

            if (request.Id.HasValue)
                restRequest.AddQueryParameter("idsProdutos", request.Id.Value);

            if (!string.IsNullOrWhiteSpace(request.IdsTabelasPrecos))
                restRequest.AddQueryParameter("idsTabelasPrecos", request.IdsTabelasPrecos);

            return await ExecuteAsync<List<ProdutoResponse>>(restRequest, token);
        }
        #endregion

        #region Estoques
        public async Task<Response<List<EstoqueResponse>>> GetEstoquesAsync(string token, EstoqueRequest request)
        {
            var resource = "apps/api/saldos-mercadorias";
            var restRequest = new RestRequest(resource, Method.Get);

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

        #region Pedido
        public async Task<Response<OperationResponse>> PostPedidoAsync(string token, PedidoRequest request)
        {
            var restRequest = new RestRequest("apps/api/pedidos", Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(request);

            return await ExecuteAsync<OperationResponse>(restRequest, token);
        }

        public async Task<Response<OperationResponse>> AlterarStatusPedidoAsync(string token, AlterarStatusPedidoRequest request)
        {
            var resource = $"apps/api/pedidos/alterar-status";
            var restRequest = new RestRequest(resource, Method.Post)
                                .AddHeader("Content-Type", "application/json")
                                .AddJsonBody(request);
            var operationRespnse = await ExecuteAsync<OperationResponse>(restRequest, token);

            return operationRespnse;
        }

        public async Task<Response<OperationResponse>> CancelarPedidoAsync(string token, long pedidoErpId)
        {
            var resource = $"apps/api/pedidos/{pedidoErpId}/cancelar";
            var restRequest = new RestRequest(resource, Method.Post);

            return await ExecuteAsync<OperationResponse>(restRequest, token);
        }

        #endregion

        #region WebhookRegister
        public async Task<Response<OperationResponse>> RegisterWebhookAsync(string token, WebhookRequest payload, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest(_webHookEnpoint, Method.Post)
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(payload);

            return await ExecuteAsync<OperationResponse>(request, token);
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
                        var singleItem = JsonConvert.DeserializeObject(content, itemType, DefaultJsonSettings);

                        var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType))!;
                        if (singleItem != null)
                            list.Add(singleItem);

                        return new Response<T>((T)list) { StatusCode = response.StatusCode };
                    }
                }
                var result = JsonConvert.DeserializeObject<T>(content);
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
                var badRequestResponse = JsonConvert.DeserializeObject<BadRequestResponse>(response.Content!);

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
