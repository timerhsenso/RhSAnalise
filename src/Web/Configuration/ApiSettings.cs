// =============================================================================
// RHSENSOERP WEB - API SETTINGS
// =============================================================================
// Arquivo: src/Web/Configuration/ApiSettings.cs
// Descrição: Classe de configuração para conexão com a API
// Versão: 1.0
// Data: 25/11/2025
// =============================================================================

namespace RhSensoERP.Web.Configuration;

/// <summary>
/// Configurações de conexão com a API REST.
/// Mapeada a partir da seção "ApiSettings" do appsettings.json.
/// </summary>
public sealed class ApiSettings
{
    /// <summary>
    /// URL base da API (ex: https://localhost:7193)
    /// </summary>
    /// <example>https://localhost:7193</example>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Timeout padrão para requisições em segundos.
    /// Padrão: 30 segundos
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Timeout específico para operações de autenticação em segundos.
    /// Padrão: 60 segundos (maior devido a operações de hash)
    /// </summary>
    public int AuthTimeoutSeconds { get; set; } = 60;

    /// <summary>
    /// Número de tentativas de retry em caso de falha transitória.
    /// Padrão: 3 tentativas
    /// </summary>
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Habilita logging detalhado de requisições HTTP.
    /// Padrão: true em Development, false em Production
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = true;
}
