// src/Identity/Application/Validators/Auth/LoginRequestValidator.cs
using FluentValidation;
using RhSensoERP.Identity.Application.DTOs.Auth;

namespace RhSensoERP.Identity.Application.Validators.Auth;

/// <summary>
/// Validador do request de login.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    // ✅ ARRAY ESTÁTICO - criado uma única vez
    private static readonly string[] ValidStrategies = { "Legado", "SaaS", "WindowsAD" };

    public LoginRequestValidator()
    {
        RuleFor(x => x.CdUsuario)
            .NotEmpty()
            .WithMessage("Código do usuário é obrigatório.")
            .MaximumLength(30)
            .WithMessage("Código do usuário deve ter no máximo 30 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("Senha é obrigatória.")
            .MinimumLength(1)
            .WithMessage("Senha não pode ser vazia.")
            .MaximumLength(100)
            .WithMessage("Senha deve ter no máximo 100 caracteres.");

        // ✅ CORREÇÃO: usar array estático + verificação simplificada
        RuleFor(x => x.AuthStrategy)
            .Must(strategy => string.IsNullOrWhiteSpace(strategy) || ValidStrategies.Contains(strategy))
            .WithMessage($"Estratégia de autenticação inválida. Valores válidos: {string.Join(", ", ValidStrategies)}.");
    }
}