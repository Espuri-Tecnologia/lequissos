using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Services;

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
    }
}
