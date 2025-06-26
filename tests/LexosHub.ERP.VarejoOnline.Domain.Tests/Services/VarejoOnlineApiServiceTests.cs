using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Services;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.Services
{
    public class VarejoOnlineApiServiceTests
    {
        private static VarejoOnlineApiService CreateService(VarejoOnlineApiSettings settings)
        {
            return new VarejoOnlineApiService(Options.Create(settings));
        }

        [Fact]
        public void Constructor_WithEmptyBaseUrl_ShouldThrow()
        {
            var settings = new VarejoOnlineApiSettings { BaseUrl = string.Empty };
            Assert.Throws<ArgumentNullException>(() => CreateService(settings));
        }

        [Fact]
        public async Task GetAuthUrl_ShouldReturnExpectedUrl()
        {
            var settings = new VarejoOnlineApiSettings
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
    }
}
