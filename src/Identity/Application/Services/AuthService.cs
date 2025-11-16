// src/Identity/Application/Services/AuthService.cs

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Core.Common;

// Alias para evitar confusão com o namespace BCrypt.Net
using BCryptNet = BCrypt.Net.BCrypt;

namespace RhSensoERP.Identity.Application.Services;

/// <summary>
/// Implementação do serviço de autenticação com suporte a múltiplas estratégias.
/// Responsável por login, refresh token, logout e validação de senhas.
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

        EnsureDefaultStrategiesExist();
    }

    /// <summary>
    /// Garante que estratégias padrão existem caso o appsettings não as defina.
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

        if (string.IsNullOrWhiteSpace(_authSettings.DefaultStrategy))
        {
            _authSettings.DefaultStrategy = "Legado";
            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy vazio. Definido como '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);
        }

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

    /// <summary>
    /// Autentica um usuário com credenciais e retorna tokens JWT.
    /// </summary>
    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🚀 AuthService.LoginAsync INICIADO para {CdUsuario}", request.CdUsuario);

            // Buscar usuário
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CdUsuario == request.CdUsuario, ct);

            if (usuario == null)
            {
                _logger.LogWarning("❌ LOGIN: Usuário {CdUsuario} NÃO ENCONTRADO", request.CdUsuario);
                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            _logger.LogInformation("✅ LOGIN: Usuário {CdUsuario} encontrado. FlAtivo={FlAtivo}",
                usuario.CdUsuario, usuario.FlAtivo);

            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("❌ LOGIN: Usuário {CdUsuario} INATIVO", request.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // Buscar/Criar UserSecurity
            _logger.LogInformation("🔍 LOGIN: Buscando UserSecurity para IdUsuario={IdUsuario}", usuario.Id);
            var userSecurity = await GetOrCreateUserSecurityAsync(usuario, ct);
            _logger.LogInformation("✅ LOGIN: UserSecurity obtido. Id={Id}, LockoutEnd={LockoutEnd}",
                userSecurity.Id, userSecurity.LockoutEnd);

            // Verificar lockout
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                var remainingMinutes = (userSecurity.LockoutEnd.Value - _dateTimeProvider.UtcNow).TotalMinutes;
                _logger.LogWarning("🔒 LOGIN: Conta BLOQUEADA até {LockoutEnd}", userSecurity.LockoutEnd);

                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Account locked", ct);

                return Result<AuthResponse>.Failure(
                    "ACCOUNT_LOCKED",
                    $"Conta bloqueada. Tente novamente em {Math.Ceiling(remainingMinutes)} minutos.");
            }

            // Determinar estratégia de autenticação
            var strategy = request.AuthStrategy ?? _authSettings.DefaultStrategy;
            _logger.LogInformation(
                "🔑 LOGIN: Estratégia solicitada: '{RequestedStrategy}', Default: '{DefaultStrategy}'",
                request.AuthStrategy,
                _authSettings.DefaultStrategy);

            if (!_authSettings.Strategies.TryGetValue(strategy, out var strategyConfig))
            {
                _logger.LogError(
                    "❌ LOGIN: Estratégia '{Strategy}' não encontrada. Disponíveis: {Available}",
                    strategy,
                    string.Join(", ", _authSettings.Strategies.Keys));

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

                strategy = _authSettings.DefaultStrategy;

                if (!_authSettings.Strategies.TryGetValue(strategy, out strategyConfig) || !strategyConfig.Enabled)
                {
                    return Result<AuthResponse>.Failure(
                        "AUTH_STRATEGY_DISABLED",
                        "A estratégia de autenticação solicitada está desabilitada.");
                }
            }

            // Validar senha
            _logger.LogInformation("🔐 LOGIN: Validando senha com estratégia '{Strategy}'", strategy);
            var isValidPassword = ValidatePassword(usuario, userSecurity, request.Senha, strategy);

            if (!isValidPassword)
            {
                _logger.LogWarning("❌ LOGIN: Senha INVÁLIDA para {CdUsuario}", request.CdUsuario);

                userSecurity.IncrementAccessFailedCount();

                if (userSecurity.AccessFailedCount >= _securityPolicy.MaxFailedAccessAttempts)
                {
                    var lockoutEnd = _dateTimeProvider.UtcNow.AddMinutes(_securityPolicy.LockoutDurationMinutes);
                    userSecurity.LockUntil(lockoutEnd, $"Max failed attempts ({_securityPolicy.MaxFailedAccessAttempts})");

                    _logger.LogWarning(
                        "🔒 LOGIN: Conta {CdUsuario} BLOQUEADA até {LockoutEnd} após {Attempts} tentativas",
                        usuario.CdUsuario,
                        lockoutEnd,
                        userSecurity.AccessFailedCount);
                }

                await UpdateUserSecurityInDatabaseAsync(userSecurity, ct);
                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Invalid password", ct);

                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            // Validações de segurança adicionais
            if (strategyConfig.RequireEmailConfirmation && !userSecurity.EmailConfirmed)
            {
                _logger.LogWarning("⚠️ LOGIN: E-mail não confirmado para {CdUsuario}", request.CdUsuario);
                return Result<AuthResponse>.Failure(
                    "EMAIL_NOT_CONFIRMED",
                    "E-mail não confirmado. Verifique sua caixa de entrada.");
            }

            if (strategyConfig.Require2FA && !userSecurity.TwoFactorEnabled)
            {
                _logger.LogWarning("⚠️ LOGIN: 2FA obrigatório mas não configurado para {CdUsuario}", request.CdUsuario);
                return Result<AuthResponse>.Failure(
                    "2FA_REQUIRED",
                    "Autenticação de dois fatores obrigatória. Configure 2FA antes de fazer login.");
            }

            // SUCESSO: Resetar tentativas e gerar tokens
            _logger.LogInformation("✅ LOGIN: Credenciais VÁLIDAS para {CdUsuario}", usuario.CdUsuario);

            userSecurity.ResetAccessFailedCount();
            userSecurity.RegisterSuccessfulLogin(ipAddress);

            await UpdateUserSecurityInDatabaseAsync(userSecurity, ct);
            await RegisterSuccessfulLoginAsync(userSecurity, ipAddress, userAgent, ct);

            // Gerar tokens JWT
            var accessToken = _jwtService.GenerateAccessToken(usuario, userSecurity);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                request.DeviceId,
                request.DeviceName,
                ct);

            // Mapear informações do usuário
            var userInfo = new UserInfoDto
            {
                Id = usuario.Id,
                CdUsuario = usuario.CdUsuario,
                DcUsuario = usuario.DcUsuario,
                Email = usuario.Email_Usuario,
                NoMatric = usuario.NoMatric,
                CdEmpresa = usuario.CdEmpresa,
                CdFilial = usuario.CdFilial,
                TenantId = usuario.TenantId,
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900, // 15 minutos em segundos
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("✅ LOGIN: Tokens gerados com sucesso para {CdUsuario}", usuario.CdUsuario);

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar login: {CdUsuario}", request.CdUsuario);
            return Result<AuthResponse>.Failure("LOGIN_ERROR", "Erro ao processar login. Tente novamente.");
        }
    }

    /// <summary>
    /// Renova tokens JWT usando um refresh token válido.
    /// </summary>
    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🔄 REFRESH: Validando refresh token");

            // Validar refresh token
            var userSecurity = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, ct);

            if (userSecurity == null)
            {
                _logger.LogWarning("❌ REFRESH: Token inválido ou expirado");
                return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Refresh token inválido ou expirado.");
            }

            // Buscar usuário associado
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userSecurity.IdUsuario, ct);

            if (usuario == null)
            {
                _logger.LogWarning("❌ REFRESH: Usuário não encontrado para UserSecurity {Id}", userSecurity.Id);
                return Result<AuthResponse>.Failure("USER_NOT_FOUND", "Usuário não encontrado.");
            }

            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("❌ REFRESH: Usuário {CdUsuario} INATIVO", usuario.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // Verificar lockout
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                _logger.LogWarning("🔒 REFRESH: Conta BLOQUEADA até {LockoutEnd}", userSecurity.LockoutEnd);
                return Result<AuthResponse>.Failure("ACCOUNT_LOCKED", "Conta bloqueada.");
            }

            // Revogar refresh token antigo
            await _jwtService.RevokeRefreshTokenAsync(
                request.RefreshToken,
                "Replaced by new token",
                ipAddress,
                ct);

            // Gerar novos tokens
            var newAccessToken = _jwtService.GenerateAccessToken(usuario, userSecurity);
            var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                null,
                null,
                ct);

            // Mapear informações do usuário
            var userInfo = new UserInfoDto
            {
                Id = usuario.Id,
                CdUsuario = usuario.CdUsuario,
                DcUsuario = usuario.DcUsuario,
                Email = usuario.Email_Usuario,
                NoMatric = usuario.NoMatric,
                CdEmpresa = usuario.CdEmpresa,
                CdFilial = usuario.CdFilial,
                TenantId = usuario.TenantId,
                TwoFactorEnabled = userSecurity.TwoFactorEnabled,
                MustChangePassword = userSecurity.MustChangePassword
            };

            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = "Bearer",
                ExpiresIn = 900,
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15),
                User = userInfo
            };

            _logger.LogInformation("✅ REFRESH: Tokens renovados com sucesso para {CdUsuario}", usuario.CdUsuario);

            return Result<AuthResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar refresh token");
            return Result<AuthResponse>.Failure("REFRESH_ERROR", "Erro ao processar refresh token.");
        }
    }

    /// <summary>
    /// Realiza logout revogando refresh tokens do usuário.
    /// </summary>
    public async Task<Result<bool>> LogoutAsync(
        string userId,
        LogoutRequest request,
        CancellationToken ct = default)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userIdGuid))
            {
                return Result<bool>.Failure("INVALID_USER_ID", "ID de usuário inválido.");
            }

            if (request.RevokeAllTokens)
            {
                _logger.LogInformation("🔓 LOGOUT: Revogando TODOS os tokens do usuário {UserId}", userId);

                var userSecurity = await _db.Set<UserSecurity>()
                    .FirstOrDefaultAsync(us => us.IdUsuario == userIdGuid, ct);

                if (userSecurity != null)
                {
                    await _jwtService.RevokeAllRefreshTokensAsync(
                        userSecurity.Id,
                        "User logout - all tokens",
                        ct);

                    // Regenerar security stamp para invalidar tokens JWT existentes
                    userSecurity.RegenerateSecurityStamp();
                    await _db.SaveChangesAsync(ct);

                    _logger.LogInformation("🔓 Todos os tokens do usuário foram revogados");
                }
            }
            else if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogInformation("🔓 LOGOUT: Revogando token específico para usuário {UserId}", userId);
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, "User logout", "N/A", ct);
            }

            _logger.LogInformation("✅ Logout realizado com sucesso: {UserId}", userId);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar logout: {UserId}", userId);
            return Result<bool>.Failure("LOGOUT_ERROR", "Erro ao processar logout.");
        }
    }

    /// <summary>
    /// Valida senha do usuário de acordo com a estratégia especificada.
    /// Suporta: Legado (texto plano ou BCrypt), SaaS (BCrypt) e WindowsAD.
    /// </summary>
    private bool ValidatePassword(
        Usuario usuario,
        UserSecurity userSecurity,
        string senha,
        string strategy)
    {
        if (!_authSettings.Strategies.TryGetValue(strategy, out var strategyConfig))
        {
            _logger.LogError(
                "Estratégia '{Strategy}' não encontrada em ValidatePassword. Usando Legado como fallback.",
                strategy);
            strategy = "Legado";
            strategyConfig = _authSettings.Strategies[strategy];
        }

        switch (strategy)
        {
            case "Legado":
                // 1) Se já existe PasswordHash no usuário → SEMPRE usa BCrypt
                if (!string.IsNullOrWhiteSpace(usuario.PasswordHash))
                {
                    return BCryptNet.Verify(senha, usuario.PasswordHash);
                }

                // 2) Se ainda está no modo legado (SenhaUser em texto)
                if (!string.IsNullOrWhiteSpace(usuario.SenhaUser))
                {
                    // Provisório: comparação em tempo constante para reduzir superfície de ataque
                    // Ideal: migrar para BCrypt no primeiro login bem-sucedido
                    return ConstantTimeEquals(senha, usuario.SenhaUser);
                }

                return false;

            case "SaaS":
                if (userSecurity == null || string.IsNullOrWhiteSpace(userSecurity.PasswordHash))
                {
                    return false;
                }

                return BCryptNet.Verify(senha, userSecurity.PasswordHash);

            case "WindowsAD":
                _logger.LogWarning("Autenticação WindowsAD ainda não implementada.");
                return false;

            default:
                return false;
        }
    }

    /// <summary>
    /// Valida senha do usuário (método público para compatibilidade).
    /// </summary>
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
        {
            return false;
        }

        var userSecurity = await _db.Set<UserSecurity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

        if (userSecurity == null)
        {
            return false;
        }

        return ValidatePassword(usuario, userSecurity, senha, strategy);
    }

    /// <summary>
    /// Busca ou cria UserSecurity para usuário legado (migração automática).
    /// </summary>
    private async Task<UserSecurity> GetOrCreateUserSecurityAsync(Usuario usuario, CancellationToken ct)
    {
        var userSecurity = await _db.Set<UserSecurity>()
            .FirstOrDefaultAsync(us => us.IdUsuario == usuario.Id, ct);

        if (userSecurity == null)
        {
            var passwordHash = !string.IsNullOrWhiteSpace(usuario.PasswordHash)
                ? usuario.PasswordHash
                : BCryptNet.HashPassword(usuario.SenhaUser ?? "ChangeMe@123");

            userSecurity = new UserSecurity(
                usuario.Id,
                usuario.TenantId,
                passwordHash,
                string.Empty);

            userSecurity.ConfirmEmail();

            _db.Set<UserSecurity>().Add(userSecurity);
            await _db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "UserSecurity criado automaticamente para usuário legado: {CdUsuario}",
                usuario.CdUsuario);
        }

        return userSecurity;
    }

    /// <summary>
    /// Atualiza UserSecurity no banco (lockout e tentativas de login).
    /// Usa ExecuteSqlRawAsync com parâmetros SqlParameter explícitos para máxima compatibilidade.
    /// </summary>
    private async Task UpdateUserSecurityInDatabaseAsync(UserSecurity userSecurity, CancellationToken ct)
    {
        var parameters = new[]
        {
            new SqlParameter("@AccessFailedCount", userSecurity.AccessFailedCount),
            new SqlParameter("@LockoutEnd", userSecurity.LockoutEnd.HasValue ? userSecurity.LockoutEnd.Value : DBNull.Value),
            new SqlParameter("@UpdatedAt", _dateTimeProvider.UtcNow),
            new SqlParameter("@Id", userSecurity.Id),
            new SqlParameter("@ConcurrencyStamp", userSecurity.ConcurrencyStamp)
        };

        await _db.Database.ExecuteSqlRawAsync(
            @"UPDATE dbo.SEG_UserSecurity
              SET AccessFailedCount = @AccessFailedCount,
                  LockoutEnd = @LockoutEnd,
                  UpdatedAt = @UpdatedAt
              WHERE Id = @Id AND ConcurrencyStamp = @ConcurrencyStamp;",
            parameters,
            ct);
    }

    /// <summary>
    /// Registra tentativa de login bem-sucedida no audit log.
    /// Não inclui [Id] no INSERT - SQL Server gera automaticamente via IDENTITY (bigint).
    /// </summary>
    private async Task RegisterSuccessfulLoginAsync(
        UserSecurity userSecurity,
        string ipAddress,
        string? userAgent,
        CancellationToken ct)
    {
        try
        {
            var parameters = new[]
            {
                new SqlParameter("@IdUserSecurity", userSecurity.Id),
                new SqlParameter("@IdSaaS", userSecurity.IdSaaS.HasValue ? userSecurity.IdSaaS.Value : DBNull.Value),
                new SqlParameter("@IpAddress", !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : DBNull.Value),
                new SqlParameter("@UserAgent", !string.IsNullOrWhiteSpace(userAgent) ? userAgent : DBNull.Value),
                new SqlParameter("@LoginAttemptAt", _dateTimeProvider.UtcNow)
            };

            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO [dbo].[SEG_LoginAuditLog] 
                      ([IdUserSecurity], [IdSaaS], [IsSuccess], [IpAddress], [UserAgent], [LoginAttemptAt])
                  VALUES 
                      (@IdUserSecurity, @IdSaaS, 1, @IpAddress, @UserAgent, @LoginAttemptAt);",
                parameters,
                ct);

            _logger.LogInformation("✅ LOGIN: Audit log registrado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ AUDIT: Erro ao registrar login no audit log (não crítico)");
        }
    }

    /// <summary>
    /// Registra tentativa de login falhada no audit log.
    /// Não inclui [Id] no INSERT - SQL Server gera automaticamente via IDENTITY (bigint).
    /// </summary>
    private async Task RegisterFailedLoginAsync(
        UserSecurity userSecurity,
        string ipAddress,
        string? userAgent,
        string? reason,
        CancellationToken ct)
    {
        try
        {
            var parameters = new[]
            {
                new SqlParameter("@IdUserSecurity", userSecurity.Id),
                new SqlParameter("@IdSaaS", userSecurity.IdSaaS.HasValue ? userSecurity.IdSaaS.Value : DBNull.Value),
                new SqlParameter("@IpAddress", !string.IsNullOrWhiteSpace(ipAddress) ? ipAddress : DBNull.Value),
                new SqlParameter("@UserAgent", !string.IsNullOrWhiteSpace(userAgent) ? userAgent : DBNull.Value),
                new SqlParameter("@FailureReason", !string.IsNullOrWhiteSpace(reason) ? reason : DBNull.Value),
                new SqlParameter("@LoginAttemptAt", _dateTimeProvider.UtcNow)
            };

            await _db.Database.ExecuteSqlRawAsync(
                @"INSERT INTO [dbo].[SEG_LoginAuditLog] 
                      ([IdUserSecurity], [IdSaaS], [IsSuccess], [IpAddress], [UserAgent], [FailureReason], [LoginAttemptAt])
                  VALUES 
                      (@IdUserSecurity, @IdSaaS, 0, @IpAddress, @UserAgent, @FailureReason, @LoginAttemptAt);",
                parameters,
                ct);

            _logger.LogInformation("✅ LOGIN: Falha registrada no audit log");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "⚠️ AUDIT: Erro ao registrar falha no audit log (não crítico)");
        }
    }

    /// <summary>
    /// Comparação em tempo constante para strings (usada apenas como fallback legado).
    /// </summary>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a is null || b is null)
        {
            return false;
        }

        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);

        if (aBytes.Length != bBytes.Length)
        {
            // Comparação "dummy" para consumir tempo similar e não vazar timing pelo tamanho
            CryptographicOperations.FixedTimeEquals(aBytes, aBytes);
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
