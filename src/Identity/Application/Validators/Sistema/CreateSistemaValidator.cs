using FluentValidation;
using RhSensoERP.Identity.Application.Requests.Sistema;

namespace RhSensoERP.Identity.Application.Validators.Sistema;

public sealed class CreateSistemaValidator : AbstractValidator<CreateSistemaRequest>
{
    public CreateSistemaValidator()
    {
        RuleFor(x => x.CdSistema)
            .NotEmpty().MaximumLength(10);
        RuleFor(x => x.DcSistema)
            .NotEmpty().MaximumLength(60);
    }
}
