namespace RhSensoERP.Identity.Application.Configuration;

/// <summary>
/// Configurações de estratégias de autenticação.
/// </summary>
public sealed class AuthSettings
{
    public string DefaultStrategy { get; init; } = "Legado";
    public bool AllowMultipleStrategies { get; init; } = true;
    public Dictionary<string, StrategyConfig> Strategies { get; init; } = new();
}

public sealed class StrategyConfig
{
    public bool Enabled { get; init; }
    public bool UseBCrypt { get; init; }
    public bool SyncWithUserSecurity { get; init; }
    public bool RequireEmailConfirmation { get; init; }
    public bool Require2FA { get; init; }
    public string? Domain { get; init; }
    public string? LdapPath { get; init; }
}