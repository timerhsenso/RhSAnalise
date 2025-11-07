using FluentValidation;
using RhSensoERP.Identity.Application.Requests.Sistema;

namespace RhSensoERP.Identity.Application.Validators.Sistema;

public sealed class UpdateSistemaValidator : AbstractValidator<UpdateSistemaRequest>
{
    public UpdateSistemaValidator()
    {
        RuleFor(x => x.DcSistema)
            .NotEmpty().MaximumLength(60);
    }
}
