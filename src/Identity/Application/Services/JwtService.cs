// src/Identity/Application/Services/JwtService.cs

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço JWT.
/// </summary>
public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IdentityDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        IdentityDbContext db,
        IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = jwtSettings.Value;
        _db = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public string GenerateAccessToken(
        Usuario usuario,
        UserSecurity? userSecurity = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            new("cdusuario", usuario.CdUsuario),
            new("dcusuario", usuario.DcUsuario),
            new(ClaimTypes.NameIdentifier, usuario.CdUsuario),
            new(ClaimTypes.Name, usuario.DcUsuario)
        };

        // Claims opcionais
        if (!string.IsNullOrWhiteSpace(usuario.Email_Usuario))
            claims.Add(new Claim(ClaimTypes.Email, usuario.Email_Usuario));

        if (!string.IsNullOrWhiteSpace(usuario.NoMatric))
            claims.Add(new Claim("nomatric", usuario.NoMatric));

        if (usuario.CdEmpresa.HasValue)
            claims.Add(new Claim("cdempresa", usuario.CdEmpresa.Value.ToString()));

        if (usuario.CdFilial.HasValue)
            claims.Add(new Claim("cdfilial", usuario.CdFilial.Value.ToString()));

        if (usuario.TenantId.HasValue)
            claims.Add(new Claim("tenantid", usuario.TenantId.Value.ToString()));

        // Flags de segurança
        if (userSecurity != null)
        {
            claims.Add(new Claim("twofactor_enabled",
                userSecurity.TwoFactorEnabled.ToString().ToLower()));
            claims.Add(new Claim("must_change_password",
                userSecurity.MustChangePassword.ToString().ToLower()));
            claims.Add(new Claim("email_confirmed",
                userSecurity.EmailConfirmed.ToString().ToLower()));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeProvider.UtcNow
                .AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = creds,
            TokenType = "JWT"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(
        Guid idUserSecurity,
        string ipAddress,
        string? deviceId = null,
        string? deviceName = null,
        CancellationToken ct = default)
    {
        var tokenBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);
        var token = Convert.ToBase64String(tokenBytes);

        var tokenHash = HashToken(token);
        var expiresAt = _dateTimeProvider.UtcNow
            .AddDays(_jwtSettings.RefreshTokenExpirationDays);

        var refreshToken = new RefreshToken(
            idUserSecurity,
            tokenHash,
            expiresAt,
            ipAddress,
            deviceId,
            deviceName
        );

        _db.Set<RefreshToken>().Add(refreshToken);
        await _db.SaveChangesAsync(ct);

        return token;
    }

    public async Task<UserSecurity?> ValidateRefreshTokenAsync(
        string token,
        CancellationToken ct = default)
    {
        var tokenHash = HashToken(token);

        // ✅ FIX: Expandir IsActive() para expressão SQL traduzível
        var refreshToken = await _db.Set<RefreshToken>()
            .Include(rt => rt.UserSecurity)
            .FirstOrDefaultAsync(
                rt => rt.TokenHash == tokenHash
                    && !rt.IsRevoked
                    && rt.ExpiresAt > DateTime.UtcNow,
                ct);

        return refreshToken?.UserSecurity;
    }

    public async Task RevokeRefreshTokenAsync(
        string token,
        string reason,
        string? revokedByIp = null,
        CancellationToken ct = default)
    {
        var tokenHash = HashToken(token);

        var refreshToken = await _db.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (refreshToken != null)
        {
            refreshToken.Revoke(revokedByIp, reason);
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RevokeAllRefreshTokensAsync(
        Guid idUserSecurity,
        string reason,
        CancellationToken ct = default)
    {
        // ✅ FIX: Expandir IsActive() para expressão SQL traduzível
        var tokens = await _db.Set<RefreshToken>()
            .Where(rt => rt.IdUserSecurity == idUserSecurity
                && !rt.IsRevoked
                && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.Revoke(null, reason);
        }

        await _db.SaveChangesAsync(ct);
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // ✅ Não validar expiração aqui
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(
                token,
                validationParameters,
                out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}