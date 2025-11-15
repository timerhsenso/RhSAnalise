namespace RhSensoERP.Identity.Application.Configuration;

/// <summary>
/// Configurações de JWT extraídas do appsettings.json.
/// </summary>
public sealed class JwtSettings
{
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
    public int ClockSkewMinutes { get; init; } = 5;
}