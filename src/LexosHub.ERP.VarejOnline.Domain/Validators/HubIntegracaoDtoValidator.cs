using FluentValidation;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;

namespace LexosHub.ERP.VarejOnline.Domain.Validators
{
    public class HubIntegracaoDtoValidator : AbstractValidator<HubIntegracaoDto>
    {
        public HubIntegracaoDtoValidator()
        {
            RuleFor(x => x.IntegracaoId).NotNull().WithMessage("IntegracaoId needs to be informed");
            RuleFor(x => x.Chave).NotEmpty();
        }

    }
}
