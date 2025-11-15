namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Request de login do usuário.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>Código do usuário (cdusuario).</summary>
    public string CdUsuario { get; init; } = string.Empty;

    /// <summary>Senha em texto plano.</summary>
    public string Senha { get; init; } = string.Empty;

    /// <summary>Estratégia de autenticação (opcional, usa padrão do config se não informado).</summary>
    public string? AuthStrategy { get; init; }

    /// <summary>Lembrar-me (refresh token com duração estendida).</summary>
    public bool RememberMe { get; init; }

    /// <summary>Informações do dispositivo (opcional, para auditoria).</summary>
    public string? DeviceId { get; init; }

    /// <summary>Nome do dispositivo (opcional, para auditoria).</summary>
    public string? DeviceName { get; init; }
}