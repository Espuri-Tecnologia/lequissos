using FluentValidation;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using System.Text.RegularExpressions;

namespace LexosHub.ERP.VarejoOnline.Domain.Validators
{
    public class HubIntegracaoDtoValidator : AbstractValidator<HubIntegracaoDto>
    {
        public HubIntegracaoDtoValidator()
        {
            RuleFor(x => x.IntegracaoId).NotNull().WithMessage("IntegracaoId needs to be informed");
            RuleFor(x => x.Chave).NotEmpty();
        }

        public bool ValidUrl(string url)
        {
            const string pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            var rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (!rgx.IsMatch(url)) return false;

            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;

        }
    }
}
