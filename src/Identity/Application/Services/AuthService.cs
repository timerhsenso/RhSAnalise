using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço de autenticação.
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly IdentityDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<AuthService> _logger;
    private readonly AuthSettings _authSettings;
    private readonly SecurityPolicySettings _securityPolicy;

    public AuthService(
        IdentityDbContext db,
        IJwtService jwtService,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        ILogger<AuthService> logger,
        IOptions<AuthSettings> authSettings,
        IOptions<SecurityPolicySettings> securityPolicy)
    {
        _db = db;
        _jwtService = jwtService;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _authSettings = authSettings.Value;
        _securityPolicy = securityPolicy.Value;
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Buscar usuário
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CdUsuario == request.CdUsuario, ct);

            if (usuario == null)
            {
                _logger.LogWarning("Tentativa de login com usuário inexistente: {CdUsuario}", request.CdUsuario);
                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            // 2. Verificar se usuário está ativo
            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("Tentativa de login com usuário inativo: {CdUsuario}", request.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // 3. Buscar/Criar UserSecurity
            var userSecurity = await GetOrCreateUserSecurityAsync(usuario, ct);

            // 4. Verificar lockout
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                var remainingMinutes = (userSecurity.LockoutEnd.Value - _dateTimeProvider.UtcNow).TotalMinutes;
                _logger.LogWarning(
                    "Tentativa de login com conta bloqueada: {CdUsuario}. Bloqueado até: {LockoutEnd}",
                    request.CdUsuario,
                    userSecurity.LockoutEnd);

                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Account locked", ct);

                return Result<AuthResponse>.Failure(
                    "ACCOUNT_LOCKED",
                    $"Conta bloqueada. Tente novamente em {Math.Ceiling(remainingMinutes)} minutos.");
            }

            // 5. Determinar estratégia de autenticação
            var strategy = request.AuthStrategy ?? _authSettings.DefaultStrategy;
            if (!_authSettings.Strategies.TryGetValue(strategy, out var strategyConfig) || !strategyConfig.Enabled)
            {
                strategy = _authSettings.DefaultStrategy;
                strategyConfig = _authSettings.Strategies[strategy];
            }

            // 6. Validar senha
            var isPasswordValid = await ValidatePasswordAsync(request.CdUsuario, request.Senha, strategy, ct);

            if (!isPasswordValid)
            {
                // Incrementar contador de falhas
                userSecurity.IncrementAccessFailedCount();

                // Verificar se deve bloquear
                if (userSecurity.AccessFailedCount >= _securityPolicy.MaxFailedAccessAttempts)
                {
                    var lockoutEnd = _dateTimeProvider.UtcNow.AddMinutes(_securityPolicy.LockoutDurationMinutes);
                    userSecurity.LockUntil(lockoutEnd, "Múltiplas tentativas de login falhadas");

                    _logger.LogWarning(
                        "Conta bloqueada por múltiplas tentativas falhadas: {CdUsuario}. Tentativas: {Count}",
                        request.CdUsuario,
                        userSecurity.AccessFailedCount);
                }

                await _db.SaveChangesAsync(ct);
                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Invalid password", ct);

                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            // 7. Verificar se precisa confirmar email (apenas para SaaS)
            if (strategy == "SaaS" && strategyConfig.RequireEmailConfirmation && !userSecurity.EmailConfirmed)
            {
                return Result<AuthResponse>.Failure("EMAIL_NOT_CONFIRMED", "Email não confirmado.");
            }

            // 8. Verificar 2FA
            if (userSecurity.TwoFactorEnabled && (strategy == "SaaS" && strategyConfig.Require2FA))
            {
                // TODO: Implementar fluxo 2FA completo em Sprint futura
                return Result<AuthResponse>.Failure("2FA_REQUIRED", "Autenticação de dois fatores necessária.");
            }

            // 9. Login bem-sucedido - resetar contador de falhas e atualizar último login
            userSecurity.RegisterSuccessfulLogin(ipAddress);
            usuario.LastLoginAt = _dateTimeProvider.UtcNow;
            usuario.LastIpAddress = ipAddress;
            usuario.LastUserAgent = userAgent;

            await _db.SaveChangesAsync(ct);

            // 10. Registrar sucesso no log de auditoria
            await RegisterSuccessfulLoginAsync(userSecurity, ipAddress, userAgent, ct);

            // 11. Gerar tokens
            var accessToken = _jwtService.GenerateAccessToken(usuario, userSecurity);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                request.DeviceId,
                request.DeviceName,
                ct);

            // 12. Montar UserInfoDto
            var userInfo = _mapper.Map<UserInfoDto>(usuario);

            // Criar novo DTO com flags de segurança usando 'with' expression
            userInfo = userInfo with
            {
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            // 13. Montar AuthResponse
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900, // 15 minutos em segundos
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("Login bem-sucedido: {CdUsuario}", request.CdUsuario);

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login: {CdUsuario}", request.CdUsuario);
            return Result<AuthResponse>.Failure("LOGIN_ERROR", "Erro ao processar login. Tente novamente.");
        }
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Validar refresh token
            var userSecurity = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, ct);

            if (userSecurity == null)
            {
                _logger.LogWarning("Tentativa de refresh com token inválido. IP: {IpAddress}", ipAddress);
                return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Refresh token inválido ou expirado.");
            }

            // 2. Buscar usuário
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userSecurity.IdUsuario, ct);

            if (usuario == null || usuario.FlAtivo != 'S')
            {
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "User not found or inactive", ipAddress, ct);
                return Result<AuthResponse>.Failure("USER_NOT_FOUND", "Usuário não encontrado ou inativo.");
            }

            // 3. Verificar lockout
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "Account locked", ipAddress, ct);
                return Result<AuthResponse>.Failure("ACCOUNT_LOCKED", "Conta bloqueada.");
            }

            // 4. Revogar o token antigo
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "Token rotation", ipAddress, ct);

            // 5. Gerar novos tokens
            var accessToken = _jwtService.GenerateAccessToken(usuario, userSecurity);
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                ct: ct);

            // 6. Montar UserInfoDto
            var userInfo = _mapper.Map<UserInfoDto>(usuario);

            // Criar novo DTO com flags de segurança
            userInfo = userInfo with
            {
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            // 7. Montar AuthResponse
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900,
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("Tokens renovados com sucesso: {CdUsuario}", usuario.CdUsuario);

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar tokens.");
            return Result<AuthResponse>.Failure("REFRESH_ERROR", "Erro ao renovar tokens.");
        }
    }

    public async Task<Result<bool>> LogoutAsync(
        string userId,
        LogoutRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CdUsuario == userId, ct);

            if (usuario == null)
            {
                return Result<bool>.Failure("USER_NOT_FOUND", "Usuário não encontrado.");
            }

            var userSecurity = await _db.Set<UserSecurity>()
                .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

            if (userSecurity == null)
            {
                return Result<bool>.Failure("USER_SECURITY_NOT_FOUND", "Dados de segurança não encontrados.");
            }

            if (request.RevokeAllTokens)
            {
                await _jwtService.RevokeAllRefreshTokensAsync(userSecurity.Id, "User logout (all devices)", ct);
                _logger.LogInformation("Logout realizado (todos os dispositivos): {CdUsuario}", userId);
            }
            else if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "User logout", ct: ct);
                _logger.LogInformation("Logout realizado (dispositivo específico): {CdUsuario}", userId);
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar logout: {UserId}", userId);
            return Result<bool>.Failure("LOGOUT_ERROR", "Erro ao processar logout.");
        }
    }

    public async Task<bool> ValidatePasswordAsync(
        string cdUsuario,
        string senha,
        string strategy,
        CancellationToken ct = default)
    {
        var usuario = await _db.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CdUsuario == cdUsuario, ct);

        if (usuario == null)
            return false;

        var strategyConfig = _authSettings.Strategies[strategy];

        switch (strategy)
        {
            case "Legado":
                // Suporta senha legada E UserSecurity
                if (!string.IsNullOrWhiteSpace(usuario.PasswordHash) && strategyConfig.UseBCrypt)
                {
                    // Senha moderna com BCrypt
                    return BCrypt.Net.BCrypt.Verify(senha, usuario.PasswordHash);
                }
                else if (!string.IsNullOrWhiteSpace(usuario.SenhaUser))
                {
                    // Senha legada em texto plano (temporário)
                    return usuario.SenhaUser == senha;
                }
                return false;

            case "SaaS":
                // Apenas UserSecurity com BCrypt
                var userSecurity = await _db.Set<UserSecurity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

                if (userSecurity == null)
                    return false;

                return BCrypt.Net.BCrypt.Verify(senha, userSecurity.PasswordHash);

            case "WindowsAD":
                // TODO: Implementar integração com Active Directory
                _logger.LogWarning("Autenticação WindowsAD ainda não implementada.");
                return false;

            default:
                return false;
        }
    }

    // ========================================
    // MÉTODOS PRIVADOS
    // ========================================

    private async Task<UserSecurity> GetOrCreateUserSecurityAsync(Usuario usuario, CancellationToken ct)
    {
        var userSecurity = await _db.Set<UserSecurity>()
            .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

        if (userSecurity == null)
        {
            // Criar UserSecurity se não existir (migração gradual)
            var passwordHash = !string.IsNullOrWhiteSpace(usuario.PasswordHash)
                ? usuario.PasswordHash
                : BCrypt.Net.BCrypt.HashPassword(usuario.SenhaUser ?? "ChangeMe@123");

            userSecurity = new UserSecurity(
                usuario.Id,
                usuario.TenantId,
                passwordHash,
                string.Empty); // Salt não usado com BCrypt

            userSecurity.ConfirmEmail(); // Auto-confirmar para usuários legados

            _db.Set<UserSecurity>().Add(userSecurity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "UserSecurity criado automaticamente para usuário legado: {CdUsuario}",
                usuario.CdUsuario);
        }

        return userSecurity;
    }

    private async Task RegisterSuccessfulLoginAsync(
        UserSecurity userSecurity,
        string ipAddress,
        string? userAgent,
        CancellationToken ct)
    {
        var log = new LoginAuditLog(
            userSecurity.Id,
            userSecurity.IdSaaS,
            true,
            ipAddress,
            userAgent,
            twoFactorUsed: userSecurity.TwoFactorEnabled);

        _db.Set<LoginAuditLog>().Add(log);
        await _db.SaveChangesAsync(ct);
    }

    private async Task RegisterFailedLoginAsync(
        UserSecurity userSecurity,
        string ipAddress,
        string? userAgent,
        string reason,
        CancellationToken ct)
    {
        var log = new LoginAuditLog(
            userSecurity.Id,
            userSecurity.IdSaaS,
            false,
            ipAddress,
            userAgent,
            failureReason: reason);

        _db.Set<LoginAuditLog>().Add(log);
        await _db.SaveChangesAsync(ct);
    }
}