using FluentValidation;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;

namespace LexosHub.ERP.VarejoOnline.Domain.Validators
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
