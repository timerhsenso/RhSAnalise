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
/// Implementação do serviço de autenticação com fallback de configuração.
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

        // ✅ CRÍTICO: Inicializar estratégias padrão se não configuradas
        EnsureDefaultStrategiesExist();
    }

    /// <summary>
    /// Garante que as estratégias padrão existam, mesmo se não configuradas no appsettings.json.
    /// </summary>
    private void EnsureDefaultStrategiesExist()
    {
        if (_authSettings.Strategies.Count == 0)
        {
            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: AuthSettings.Strategies vazio. Criando configurações padrão.");

            _authSettings.Strategies["Legado"] = new StrategyConfig
            {
                Enabled = true,
                UseBCrypt = false,
                SyncWithUserSecurity = true,
                RequireEmailConfirmation = false,
                Require2FA = false
            };

            _authSettings.Strategies["SaaS"] = new StrategyConfig
            {
                Enabled = true,
                UseBCrypt = true,
                SyncWithUserSecurity = false,
                RequireEmailConfirmation = true,
                Require2FA = false
            };

            _authSettings.Strategies["WindowsAD"] = new StrategyConfig
            {
                Enabled = false,
                UseBCrypt = false,
                SyncWithUserSecurity = false,
                RequireEmailConfirmation = false,
                Require2FA = false
            };

            _logger.LogInformation(
                "✅ INICIALIZAÇÃO: Estratégias padrão criadas: {Strategies}",
                string.Join(", ", _authSettings.Strategies.Keys));
        }
        else
        {
            _logger.LogInformation(
                "✅ INICIALIZAÇÃO: Estratégias carregadas do appsettings: {Strategies}",
                string.Join(", ", _authSettings.Strategies.Keys));
        }

        // Validar DefaultStrategy
        if (string.IsNullOrWhiteSpace(_authSettings.DefaultStrategy))
        {
            _authSettings.DefaultStrategy = "Legado";
            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy vazio. Definido como '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);
        }

        // Verificar se DefaultStrategy existe nas Strategies
        if (!_authSettings.Strategies.ContainsKey(_authSettings.DefaultStrategy))
        {
            var firstEnabled = _authSettings.Strategies
                .FirstOrDefault(s => s.Value.Enabled).Key ?? "Legado";

            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy '{DefaultStrategy}' não encontrada. Usando '{Fallback}'",
                _authSettings.DefaultStrategy,
                firstEnabled);

            _authSettings.DefaultStrategy = firstEnabled;
        }
    }

    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🚀 AuthService.LoginAsync INICIADO para {CdUsuario}", request.CdUsuario);

            // 1. Buscar usuário
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CdUsuario == request.CdUsuario, ct);

            if (usuario == null)
            {
                _logger.LogWarning("❌ LOGIN: Usuário {CdUsuario} NÃO ENCONTRADO", request.CdUsuario);
                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            _logger.LogInformation("✅ LOGIN: Usuário {CdUsuario} encontrado. FlAtivo={FlAtivo}", usuario.CdUsuario, usuario.FlAtivo);

            // 2. Verificar se usuário está ativo
            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("❌ LOGIN: Usuário {CdUsuario} INATIVO", request.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // 3. Buscar/Criar UserSecurity
            _logger.LogInformation("🔍 LOGIN: Buscando UserSecurity para IdUsuario={IdUsuario}", usuario.Id);
            var userSecurity = await GetOrCreateUserSecurityAsync(usuario, ct);
            _logger.LogInformation("✅ LOGIN: UserSecurity obtido. Id={Id}, LockoutEnd={LockoutEnd}", userSecurity.Id, userSecurity.LockoutEnd);

            // 4. Verificar lockout
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                var remainingMinutes = (userSecurity.LockoutEnd.Value - _dateTimeProvider.UtcNow).TotalMinutes;
                _logger.LogWarning("🔒 LOGIN: Conta BLOQUEADA até {LockoutEnd}", userSecurity.LockoutEnd);

                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Account locked", ct);

                return Result<AuthResponse>.Failure(
                    "ACCOUNT_LOCKED",
                    $"Conta bloqueada. Tente novamente em {Math.Ceiling(remainingMinutes)} minutos.");
            }

            // 5. Determinar estratégia de autenticação com validação
            var strategy = request.AuthStrategy ?? _authSettings.DefaultStrategy;
            _logger.LogInformation(
                "🔑 LOGIN: Estratégia solicitada: '{RequestedStrategy}', Default: '{DefaultStrategy}'",
                request.AuthStrategy,
                _authSettings.DefaultStrategy);

            // ✅ VALIDAÇÃO: Verificar se a estratégia existe
            if (!_authSettings.Strategies.TryGetValue(strategy, out var strategyConfig))
            {
                _logger.LogError(
                    "❌ LOGIN: Estratégia '{Strategy}' não encontrada. Disponíveis: {Available}",
                    strategy,
                    string.Join(", ", _authSettings.Strategies.Keys));

                // Fallback para estratégia padrão
                if (_authSettings.Strategies.TryGetValue(_authSettings.DefaultStrategy, out strategyConfig))
                {
                    strategy = _authSettings.DefaultStrategy;
                    _logger.LogWarning("🔄 LOGIN: Usando estratégia padrão '{DefaultStrategy}'", strategy);
                }
                else
                {
                    return Result<AuthResponse>.Failure(
                        "INVALID_AUTH_STRATEGY",
                        "Nenhuma estratégia de autenticação disponível. Contate o administrador.");
                }
            }

            if (!strategyConfig!.Enabled)
            {
                _logger.LogWarning("⚠️ LOGIN: Estratégia '{Strategy}' está DESABILITADA", strategy);

                // Fallback para estratégia padrão
                strategy = _authSettings.DefaultStrategy;

                if (!_authSettings.Strategies.TryGetValue(strategy, out strategyConfig) || !strategyConfig.Enabled)
                {
                    return Result<AuthResponse>.Failure(
                        "NO_AUTH_STRATEGY_AVAILABLE",
                        "Nenhuma estratégia de autenticação disponível. Contate o administrador.");
                }

                _logger.LogInformation("🔄 LOGIN: Usando estratégia padrão '{DefaultStrategy}'", strategy);
            }

            // 6. Validar senha
            _logger.LogInformation("🔐 LOGIN: Validando senha com estratégia '{Strategy}'", strategy);
            var isPasswordValid = await ValidatePasswordAsync(request.CdUsuario, request.Senha, strategy, ct);

            if (!isPasswordValid)
            {
                _logger.LogWarning("❌ LOGIN: SENHA INVÁLIDA para {CdUsuario}", request.CdUsuario);

                // Incrementar contador de falhas
                userSecurity.IncrementAccessFailedCount();

                // Verificar se deve bloquear
                if (userSecurity.AccessFailedCount >= _securityPolicy.MaxFailedAccessAttempts)
                {
                    var lockoutEnd = _dateTimeProvider.UtcNow.AddMinutes(_securityPolicy.LockoutDurationMinutes);
                    userSecurity.LockUntil(lockoutEnd, "Múltiplas tentativas de login falhadas");

                    _logger.LogWarning(
                        "🔒 LOGIN: Conta bloqueada. Tentativas: {Count}",
                        userSecurity.AccessFailedCount);
                }

                await _db.SaveChangesAsync(ct);
                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Invalid password", ct);

                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            // 7. Verificar se precisa confirmar email (apenas para SaaS)
            if (strategy == "SaaS" && strategyConfig.RequireEmailConfirmation && !userSecurity.EmailConfirmed)
            {
                _logger.LogWarning("📧 LOGIN: Email não confirmado para {CdUsuario}", request.CdUsuario);
                return Result<AuthResponse>.Failure("EMAIL_NOT_CONFIRMED", "Email não confirmado.");
            }

            // 8. Verificar 2FA
            if (userSecurity.TwoFactorEnabled && (strategy == "SaaS" && strategyConfig.Require2FA))
            {
                _logger.LogInformation("🔐 LOGIN: 2FA necessário para {CdUsuario}", request.CdUsuario);
                return Result<AuthResponse>.Failure("2FA_REQUIRED", "Autenticação de dois fatores necessária.");
            }

            // 9. Login bem-sucedido - resetar contador de falhas e atualizar último login
            userSecurity.RegisterSuccessfulLogin(ipAddress);
            usuario.LastLoginAt = _dateTimeProvider.UtcNow;
            usuario.LastIpAddress = ipAddress;
            usuario.LastUserAgent = userAgent;

            await _db.SaveChangesAsync(ct);
            await RegisterSuccessfulLoginAsync(userSecurity, ipAddress, userAgent, ct);

            // 10. Gerar tokens
            _logger.LogInformation("🎫 LOGIN: Gerando tokens JWT para {CdUsuario}", request.CdUsuario);
            var accessToken = _jwtService.GenerateAccessToken(usuario, userSecurity);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                request.DeviceId,
                request.DeviceName,
                ct);

            // 11. Montar UserInfoDto
            var userInfo = _mapper.Map<UserInfoDto>(usuario);
            userInfo = userInfo with
            {
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            // 12. Montar AuthResponse
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900,
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("✅ LOGIN: Sucesso para {CdUsuario}", request.CdUsuario);

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar login: {CdUsuario}", request.CdUsuario);
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
            // 1. Validar refresh token e obter UserSecurity
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

        // ✅ SEGURANÇA: Verificar se estratégia existe antes de acessar
        if (!_authSettings.Strategies.TryGetValue(strategy, out var strategyConfig))
        {
            _logger.LogError(
                "Estratégia '{Strategy}' não encontrada em ValidatePasswordAsync. Usando Legado como fallback.",
                strategy);
            strategy = "Legado";
            strategyConfig = _authSettings.Strategies[strategy];
        }

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