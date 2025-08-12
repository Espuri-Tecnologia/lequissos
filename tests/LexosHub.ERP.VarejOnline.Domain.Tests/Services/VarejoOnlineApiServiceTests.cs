using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Services;
using Microsoft.Extensions.Options;
using RestSharp;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Services
{
    public class VarejOnlineApiServiceTests
    {
        private static VarejOnlineApiService CreateService(VarejOnlineApiSettings settings)
        {
            return new VarejOnlineApiService(Options.Create(settings));
        }

        [Fact]
        public void Constructor_WithEmptyBaseUrl_ShouldThrow()
        {
            var settings = new VarejOnlineApiSettings { BaseUrl = string.Empty };
            Assert.Throws<ArgumentNullException>(() => CreateService(settings));
        }

        [Fact]
        public async Task GetAuthUrl_ShouldReturnExpectedUrl()
        {
            var settings = new VarejOnlineApiSettings
            {
                BaseUrl = "https://api/",
                OAuthUrl = "oauth?",
                ClientId = "id",
                OAuthRedirectUrl = "redir"
            };
            var service = CreateService(settings);

            var url = await service.GetAuthUrl();

            Assert.Equal("https://api/oauth?client_id=id&redirect_uri=redir", url);
        }

        [Fact]
        public async Task GetProdutosAsync_WithProdutoBase_ShouldIncludeQueryParameter()
        {
            var settings = new VarejOnlineApiSettings { BaseUrl = "https://api/" };
            var service = CreateService(settings);

            var client = new TestRestClient();
            typeof(VarejOnlineApiService)
                .GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(service, client);

            var request = new ProdutoRequest { ProdutoBase = 10 };
            await service.GetProdutosAsync("token", request);

            var uri = client.BuildUri(client.LastRequest!);
            Assert.Contains("produtoBase=10", uri.Query);
        }

        [Fact]
        public async Task AlterarStatusPedidoAsync_ShouldBuildUrlAndHandleResponses()
        {
            var settings = new VarejOnlineApiSettings { BaseUrl = "https://api/" };
            var service = CreateService(settings);

            var client = new TestRestClient();
            typeof(VarejOnlineApiService)
                .GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(service, client);

            var response = await service.AlterarStatusPedidoAsync("token", 123, "novo");

            var uri = client.BuildUri(client.LastRequest!);
            Assert.Equal("/apps/api/pedidos/123/status/novo", uri.AbsolutePath);
            Assert.Contains("token=token", uri.Query);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            client = new TestRestClient(HttpStatusCode.Conflict, "{}");
            typeof(VarejOnlineApiService)
                .GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(service, client);

            response = await service.AlterarStatusPedidoAsync("token", 1, "novo");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.False(response.IsSuccess);
        }

        private class TestRestClient : RestClient
        {
            public RestRequest? LastRequest { get; private set; }
            private readonly HttpStatusCode _statusCode;
            private readonly string _content;

            public TestRestClient(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "[]") : base("https://test")
            {
                _statusCode = statusCode;
                _content = content;
            }

            public new Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
            {
                LastRequest = request;
                return Task.FromResult(new RestResponse
                {
                    StatusCode = _statusCode,
                    Content = _content
                });
            }
        }
    }
}
