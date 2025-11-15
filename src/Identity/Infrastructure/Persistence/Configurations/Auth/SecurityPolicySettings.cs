namespace RhSensoERP.Identity.Application.Configuration;

/// <summary>
/// Configurações de política de segurança extraídas do appsettings.json.
/// </summary>
public sealed class SecurityPolicySettings
{
    public int PasswordMinLength { get; init; } = 8;
    public bool PasswordRequireDigit { get; init; } = true;
    public bool PasswordRequireUppercase { get; init; } = true;
    public bool PasswordRequireLowercase { get; init; } = true;
    public bool PasswordRequireNonAlphanumeric { get; init; } = true;
    public int MaxFailedAccessAttempts { get; init; } = 5;
    public int LockoutDurationMinutes { get; init; } = 30;
    public int ResetFailedCountAfterMinutes { get; init; } = 15;
}