using FluentValidation;
using RhSensoERP.Identity.Application.DTOs.Auth;

namespace RhSensoERP.Identity.Application.Validators.Auth;

/// <summary>
/// Validador do request de login.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.CdUsuario)
            .NotEmpty().WithMessage("Código do usuário é obrigatório.")
            .MaximumLength(30).WithMessage("Código do usuário deve ter no máximo 30 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MaximumLength(100).WithMessage("Senha inválida.");

        RuleFor(x => x.AuthStrategy)
            .Must(x => x == null || new[] { "Legado", "SaaS", "WindowsAD" }.Contains(x))
            .WithMessage("Estratégia de autenticação inválida. Valores válidos: Legado, SaaS, WindowsAD.");
    }
}