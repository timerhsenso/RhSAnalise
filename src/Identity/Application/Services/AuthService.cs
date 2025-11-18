// ============================================================================
// ARQUIVO ALTERADO - SUBSTITUIR: src/Identity/Application/Services/AuthService.cs
// ============================================================================

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
    private readonly ITenantContext _tenantContext; // ✅ NOVO - FASE 1
    private readonly IPermissaoService _permissaoService; // ✅ NOVO - FASE 2
    private readonly IActiveDirectoryService _activeDirectoryService; // ✅ NOVO - FASE 3
    private readonly ILogger<AuthService> _logger;
    private readonly AuthSettings _authSettings;
    private readonly SecurityPolicySettings _securityPolicy;

    public AuthService(
        IdentityDbContext db,
        IJwtService jwtService,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider,
        ITenantContext tenantContext, // ✅ NOVO - FASE 1
        IPermissaoService permissaoService, // ✅ NOVO - FASE 2
        IActiveDirectoryService activeDirectoryService, // ✅ NOVO - FASE 3
        ILogger<AuthService> logger,
        IOptions<AuthSettings> authSettings,
        IOptions<SecurityPolicySettings> securityPolicy)
    {
        _db = db;
        _jwtService = jwtService;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
        _tenantContext = tenantContext; // ✅ NOVO - FASE 1
        _permissaoService = permissaoService; // ✅ NOVO - FASE 2
        _activeDirectoryService = activeDirectoryService; // ✅ NOVO - FASE 3
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

            _authSettings.Strategies["Legacy"] = new StrategyConfig
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

            _authSettings.Strategies["ADWin"] = new StrategyConfig
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
            _authSettings.DefaultStrategy = "Legacy";
            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy vazio. Definido como '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);
        }

        if (!_authSettings.Strategies.ContainsKey(_authSettings.DefaultStrategy))
        {
            var firstEnabled = _authSettings.Strategies
                .FirstOrDefault(s => s.Value.Enabled).Key ?? "Legacy";

            _logger.LogWarning(
                "⚠️ INICIALIZAÇÃO: DefaultStrategy '{DefaultStrategy}' não encontrada. Usando '{Fallback}'",
                _authSettings.DefaultStrategy,
                firstEnabled);

            _authSettings.DefaultStrategy = firstEnabled;
        }
    }

    /// <summary>
    /// Autentica um usuário com credenciais e retorna tokens JWT.
    /// ✅ REFATORADO - FASE 1: Implementa lógica correta conforme documento de requisitos.
    /// </summary>
    public async Task<Result<AuthResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent = null,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("🚀 AuthService.LoginAsync INICIADO para {LoginIdentifier}", request.LoginIdentifier);

            // ============================================================================
            // ETAPA 1: RESOLVER TENANT
            // ============================================================================
            var tenantId = _tenantContext.TenantId;
            Guid? tenantGuid = null;

            if (!string.IsNullOrEmpty(tenantId) && Guid.TryParse(tenantId, out var parsedTenantId))
            {
                tenantGuid = parsedTenantId;
                _logger.LogInformation("✅ ETAPA 1: Tenant resolvido - TenantId: {TenantId}", tenantId);
            }
            else
            {
                _logger.LogWarning("⚠️ ETAPA 1: Tenant não resolvido. Usando configuração global.");
            }

            // ============================================================================
            // ETAPA 2: DETERMINAR AUTHMODE
            // ============================================================================
            var authMode = await DeterminarAuthModeAsync(tenantGuid, ct);
            _logger.LogInformation("✅ ETAPA 2: AuthMode determinado: '{AuthMode}'", authMode);

            // ============================================================================
            // ETAPA 3: LOCALIZAR USUÁRIO
            // ============================================================================
            var usuario = await LocalizarUsuarioAsync(request.LoginIdentifier, authMode, ct);

            if (usuario == null)
            {
                _logger.LogWarning("❌ ETAPA 3: Usuário '{LoginIdentifier}' NÃO ENCONTRADO", request.LoginIdentifier);
                return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Usuário ou senha inválidos.");
            }

            _logger.LogInformation("✅ ETAPA 3: Usuário encontrado - CdUsuario: {CdUsuario}, FlAtivo: {FlAtivo}",
                usuario.CdUsuario, usuario.FlAtivo);

            // Verificar se usuário está ativo
            if (usuario.FlAtivo != 'S')
            {
                _logger.LogWarning("❌ LOGIN: Usuário {CdUsuario} INATIVO", usuario.CdUsuario);
                return Result<AuthResponse>.Failure("USER_INACTIVE", "Usuário inativo.");
            }

            // ============================================================================
            // ETAPA 4: CARREGAR SEGURANÇA MODERNA
            // ============================================================================
            _logger.LogInformation("🔍 ETAPA 4: Buscando UserSecurity para IdUsuario={IdUsuario}", usuario.Id);
            var userSecurity = await GetOrCreateUserSecurityAsync(usuario, ct);
            _logger.LogInformation("✅ ETAPA 4: UserSecurity obtido. Id={Id}, LockoutEnd={LockoutEnd}",
                userSecurity.Id, userSecurity.LockoutEnd);

            // ============================================================================
            // ETAPA 5: VERIFICAR LOCKOUT
            // ============================================================================
            if (userSecurity.LockoutEnd.HasValue && userSecurity.LockoutEnd > _dateTimeProvider.UtcNow)
            {
                var remainingMinutes = (userSecurity.LockoutEnd.Value - _dateTimeProvider.UtcNow).TotalMinutes;
                _logger.LogWarning("🔒 ETAPA 5: Conta BLOQUEADA até {LockoutEnd}", userSecurity.LockoutEnd);

                await RegisterFailedLoginAsync(userSecurity, ipAddress, userAgent, "Account locked", ct);

                return Result<AuthResponse>.Failure(
                    "ACCOUNT_LOCKED",
                    $"Conta bloqueada. Tente novamente em {Math.Ceiling(remainingMinutes)} minutos.");
            }

            _logger.LogInformation("✅ ETAPA 5: Lockout verificado - Conta não está bloqueada");

            // Obter configuração da estratégia
            if (!_authSettings.Strategies.TryGetValue(authMode, out var strategyConfig))
            {
                _logger.LogError(
                    "❌ LOGIN: Estratégia '{AuthMode}' não encontrada. Disponíveis: {Available}",
                    authMode,
                    string.Join(", ", _authSettings.Strategies.Keys));

                return Result<AuthResponse>.Failure(
                    "INVALID_AUTH_STRATEGY",
                    "Modo de autenticação inválido. Contate o administrador.");
            }

            if (!strategyConfig.Enabled)
            {
                _logger.LogWarning("⚠️ LOGIN: Estratégia '{AuthMode}' está DESABILITADA", authMode);
                return Result<AuthResponse>.Failure(
                    "AUTH_STRATEGY_DISABLED",
                    "O modo de autenticação está desabilitado.");
            }

            // ============================================================================
            // ETAPA 6: VALIDAR CREDENCIAIS
            // ============================================================================
            _logger.LogInformation("🔐 ETAPA 6: Validando senha com estratégia '{AuthMode}'", authMode);
            var isValidPassword = ValidatePassword(usuario, userSecurity, request.Senha, authMode);

            if (!isValidPassword)
            {
                _logger.LogWarning("❌ ETAPA 6: Senha INVÁLIDA para {CdUsuario}", usuario.CdUsuario);

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

            _logger.LogInformation("✅ ETAPA 6: Credenciais VÁLIDAS");

            // Validações de segurança adicionais
            if (strategyConfig.RequireEmailConfirmation && !userSecurity.EmailConfirmed)
            {
                _logger.LogWarning("⚠️ LOGIN: E-mail não confirmado para {CdUsuario}", usuario.CdUsuario);
                return Result<AuthResponse>.Failure(
                    "EMAIL_NOT_CONFIRMED",
                    "E-mail não confirmado. Verifique sua caixa de entrada.");
            }

            if (strategyConfig.Require2FA && !userSecurity.TwoFactorEnabled)
            {
                _logger.LogWarning("⚠️ LOGIN: 2FA obrigatório mas não configurado para {CdUsuario}", usuario.CdUsuario);
                return Result<AuthResponse>.Failure(
                    "2FA_REQUIRED",
                    "Autenticação de dois fatores obrigatória. Configure 2FA antes de fazer login.");
            }

            // ============================================================================
            // ETAPA 7: REGISTRAR AUDITORIA
            // ETAPA 8: RESET/INCREMENTO DE TENTATIVAS
            // ============================================================================
            _logger.LogInformation("✅ ETAPA 7-8: Resetando tentativas e registrando auditoria");

            userSecurity.ResetAccessFailedCount();
            userSecurity.RegisterSuccessfulLogin(ipAddress);

            await UpdateUserSecurityInDatabaseAsync(userSecurity, ct);
            await RegisterSuccessfulLoginAsync(userSecurity, ipAddress, userAgent, ct);

            // ============================================================================
            // ETAPA 9: CARREGAR PERMISSÕES
            // ============================================================================
            _logger.LogInformation("🔑 ETAPA 9: Carregando permissões do usuário");

            UserPermissionsDto? permissions = null;

            try
            {
                permissions = await _permissaoService.CarregarPermissoesAsync(
                    usuario.CdUsuario,
                    cdSistema: null, // null = carregar de todos os sistemas
                    ct);

                _logger.LogInformation(
                    "✅ ETAPA 9: Permissões carregadas - Grupos: {Grupos}, Funções: {Funcoes}, Botões: {Botoes}",
                    permissions.Grupos.Count,
                    permissions.Funcoes.Count,
                    permissions.Botoes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "⚠️ ETAPA 9: Erro ao carregar permissões. Login continuará sem permissões.");

                // Não bloquear o login se houver erro ao carregar permissões
                // O usuário consegue logar, mas sem permissões no token
                permissions = null;
            }

            // ============================================================================
            // ETAPA 10: CRIAR SESSÃO / TOKEN / CLAIMS
            // ============================================================================
            _logger.LogInformation("✅ ETAPA 10: Gerando tokens JWT");

            var accessToken = _jwtService.GenerateAccessToken(usuario, userSecurity, permissions);
            var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                userSecurity.Id,
                ipAddress,
                request.DeviceId,
                request.DeviceName,
                null, // expirationDays
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
            _logger.LogError(ex, "❌ Erro ao processar login: {LoginIdentifier}", request.LoginIdentifier);
            return Result<AuthResponse>.Failure("LOGIN_ERROR", "Erro ao processar login. Tente novamente.");
        }
    }

    // ============================================================================
    // MÉTODOS AUXILIARES - NOVOS (FASE 1)
    // ============================================================================

    /// <summary>
    /// ✅ NOVO - FASE 1: Determina o AuthMode consultando SEG_SecurityPolicy do banco.
    /// Ordem de prioridade:
    /// 1. SEG_SecurityPolicy.AuthMode (por tenant)
    /// 2. DefaultStrategy (appsettings.json)
    /// </summary>
    private async Task<string> DeterminarAuthModeAsync(Guid? tenantId, CancellationToken ct)
    {
        try
        {
            // Consultar política de segurança do tenant
            SecurityPolicy? securityPolicy = null;

            if (tenantId.HasValue)
            {
                securityPolicy = await _db.Set<SecurityPolicy>()
                    .AsNoTracking()
                    .Where(sp => sp.IdSaaS == tenantId && sp.IsActive)
                    .FirstOrDefaultAsync(ct);
            }

            // Se não encontrou por tenant, buscar política global (IdSaaS = null)
            if (securityPolicy == null)
            {
                securityPolicy = await _db.Set<SecurityPolicy>()
                    .AsNoTracking()
                    .Where(sp => sp.IdSaaS == null && sp.IsActive)
                    .FirstOrDefaultAsync(ct);
            }

            // Se encontrou política e tem AuthMode definido, usar
            if (securityPolicy != null && !string.IsNullOrWhiteSpace(securityPolicy.AuthMode))
            {
                _logger.LogInformation(
                    "✅ AuthMode obtido do banco: '{AuthMode}' (Tenant: {TenantId})",
                    securityPolicy.AuthMode,
                    tenantId?.ToString() ?? "Global");

                return securityPolicy.AuthMode;
            }

            // Fallback: usar configuração padrão do appsettings
            _logger.LogInformation(
                "⚠️ AuthMode não encontrado no banco. Usando DefaultStrategy: '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);

            return _authSettings.DefaultStrategy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao determinar AuthMode. Usando DefaultStrategy: '{DefaultStrategy}'",
                _authSettings.DefaultStrategy);

            return _authSettings.DefaultStrategy;
        }
    }

    /// <summary>
    /// ✅ NOVO - FASE 1: Localiza usuário por email ou cdusuario conforme o AuthMode.
    /// - Legacy: busca por cdusuario OU email
    /// - SaaS: busca SOMENTE por email
    /// - ADWin: busca por cdusuario
    /// </summary>
    private async Task<Usuario?> LocalizarUsuarioAsync(
        string loginIdentifier,
        string authMode,
        CancellationToken ct)
    {
        Usuario? usuario = null;

        switch (authMode)
        {
            case "SaaS":
                // SaaS: buscar SOMENTE por email
                _logger.LogDebug("🔍 Buscando usuário por EMAIL (modo SaaS): {Email}", loginIdentifier);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email_Usuario == loginIdentifier, ct);
                break;

            case "Legacy":
                // Legacy: buscar por cdusuario OU email
                _logger.LogDebug("🔍 Buscando usuário por CDUSUARIO ou EMAIL (modo Legacy): {Identifier}", loginIdentifier);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u =>
                        u.CdUsuario == loginIdentifier ||
                        u.Email_Usuario == loginIdentifier, ct);
                break;

            case "ADWin":
                // ADWin: buscar por cdusuario (deve corresponder ao AD)
                _logger.LogDebug("🔍 Buscando usuário por CDUSUARIO (modo ADWin): {CdUsuario}", loginIdentifier);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.CdUsuario == loginIdentifier, ct);
                break;

            default:
                _logger.LogWarning("⚠️ AuthMode desconhecido: '{AuthMode}'. Usando busca padrão por cdusuario.", authMode);
                usuario = await _db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.CdUsuario == loginIdentifier, ct);
                break;
        }

        return usuario;
    }

    // ============================================================================
    // MÉTODOS AUXILIARES - MANTIDOS DO CÓDIGO ORIGINAL
    // ============================================================================

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
            // Validar refresh token e buscar UserSecurity
            var isValid = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, Guid.Empty, ct);

            if (!isValid)
            {
                _logger.LogWarning("❌ REFRESH: Token inválido ou expirado");
                return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Refresh token inválido ou expirado.");
            }

            // Buscar UserSecurity pelo token
            var userSecurity = await _db.Set<UserSecurity>()
                // Usuario será buscado separadamente
                .FirstOrDefaultAsync(us => us.RefreshTokens.Any(rt => rt.TokenHash == request.RefreshToken), ct);



            // Buscar usuário associado ao UserSecurity
            if (userSecurity == null)
            {
                _logger.LogWarning("❌ REFRESH: UserSecurity não encontrado");
                return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Refresh token inválido.");
            }

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
            // Carregar permissões para o novo token
            UserPermissionsDto? permissions = null;
            try
            {
                permissions = await _permissaoService.CarregarPermissoesAsync(usuario.CdUsuario, null, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Erro ao carregar permissões no refresh token");
            }

            var newAccessToken = _jwtService.GenerateAccessToken(usuario, userSecurity, permissions);
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
                    await _jwtService.RevokeAllUserTokensAsync(
                        userSecurity.Id,
                        "unknown", // ipAddress não disponível no contexto
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
    /// Suporta: Legacy (texto plano ou BCrypt), SaaS (BCrypt) e ADWin.
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
                "Estratégia '{Strategy}' não encontrada em ValidatePassword. Usando Legacy como fallback.",
                strategy);
            strategy = "Legacy";
            strategyConfig = _authSettings.Strategies[strategy];
        }

        switch (strategy)
        {
            case "Legacy":
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

            case "ADWin":
                // ✅ FASE 3: Autenticação Active Directory
                _logger.LogInformation("🔐 ADWIN: Iniciando autenticação Active Directory");

                // Verificar se AD está disponível
                if (!_activeDirectoryService.IsAvailable())
                {
                    _logger.LogWarning("⚠️ ADWIN: AD não está disponível, tentando fallback para senha local");

                    // Fallback 1: Tentar SaaS
                    if (userSecurity != null && !string.IsNullOrWhiteSpace(userSecurity.PasswordHash))
                    {
                        _logger.LogInformation("🔄 ADWIN: Tentando fallback para SAAS");
                        return ValidatePassword(usuario, userSecurity, senha, "SaaS");
                    }

                    // Fallback 2: Tentar Legacy
                    if (!string.IsNullOrWhiteSpace(usuario.PasswordHash))
                    {
                        _logger.LogInformation("🔄 ADWIN: Tentando fallback para LEGACY (PasswordHash)");
                        return BCryptNet.Verify(senha, usuario.PasswordHash);
                    }

                    if (!string.IsNullOrWhiteSpace(usuario.SenhaUser))
                    {
                        _logger.LogInformation("🔄 ADWIN: Tentando fallback para LEGACY (SenhaUser)");
                        return ConstantTimeEquals(senha, usuario.SenhaUser);
                    }

                    _logger.LogError("❌ ADWIN: AD indisponível e sem fallback configurado");
                    return false;
                }

                // Autenticar no AD usando cdusuario (síncrono pois ValidatePassword é síncrono)
                try
                {
                    var isAdValid = _activeDirectoryService.AuthenticateAsync(
                        usuario.CdUsuario,
                        senha,
                        domain: null,
                        CancellationToken.None).GetAwaiter().GetResult();

                    if (isAdValid)
                    {
                        _logger.LogInformation("✅ ADWIN: Autenticação AD bem-sucedida para {CdUsuario}", usuario.CdUsuario);
                    }
                    else
                    {
                        _logger.LogWarning("❌ ADWIN: Autenticação AD falhou para {CdUsuario}", usuario.CdUsuario);
                    }

                    return isAdValid;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ ADWIN: Erro ao autenticar no AD");
                    return false;
                }

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
