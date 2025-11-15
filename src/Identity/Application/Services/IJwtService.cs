using System.Security.Claims;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Serviço para geração e validação de JWT tokens.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Gera um access token JWT para o usuário.
    /// </summary>
    string GenerateAccessToken(Usuario usuario, UserSecurity? userSecurity = null);

    /// <summary>
    /// Gera um refresh token e persiste no banco.
    /// </summary>
    Task<string> GenerateRefreshTokenAsync(
        Guid idUserSecurity,
        string ipAddress,
        string? deviceId = null,
        string? deviceName = null,
        CancellationToken ct = default);

    /// <summary>
    /// Valida um refresh token e retorna o UserSecurity associado.
    /// </summary>
    Task<UserSecurity?> ValidateRefreshTokenAsync(string token, CancellationToken ct = default);

    /// <summary>
    /// Revoga um refresh token específico.
    /// </summary>
    Task RevokeRefreshTokenAsync(string token, string reason, string? revokedByIp = null, CancellationToken ct = default);

    /// <summary>
    /// Revoga todos os refresh tokens de um usuário.
    /// </summary>
    Task RevokeAllRefreshTokensAsync(Guid idUserSecurity, string reason, CancellationToken ct = default);

    /// <summary>
    /// Extrai claims de um token JWT (sem validar assinatura).
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}