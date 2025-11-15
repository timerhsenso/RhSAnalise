// RhSensoERP.Identity/Domain/Entities/SecurityPolicy.cs

namespace RhSensoERP.Identity.Domain.Entities;

/// <summary>
/// Políticas de segurança configuráveis por tenant ou globais.
/// Define regras de senha, lockout, sessão aplicáveis a grupos de usuários.
/// </summary>
public class SecurityPolicy
{
    // ========================================
    // IDENTIDADE
    // ========================================

    public Guid Id { get; private set; }
    public Guid? IdSaaS { get; private set; }
    public string PolicyName { get; private set; } = string.Empty;

    // ========================================
    // CONFIGURAÇÕES DE SENHA
    // ========================================

    public int PasswordMinLength { get; private set; } = 8;
    public bool PasswordRequireDigit { get; private set; } = true;
    public bool PasswordRequireUppercase { get; private set; } = true;
    public bool PasswordRequireLowercase { get; private set; } = true;
    public bool PasswordRequireNonAlphanumeric { get; private set; } = true;
    public int? PasswordExpirationDays { get; private set; } = 90;
    public int PasswordHistoryCount { get; private set; } = 5;

    // ========================================
    // CONFIGURAÇÕES DE LOCKOUT
    // ========================================

    public int MaxFailedAccessAttempts { get; private set; } = 5;
    public int LockoutDurationMinutes { get; private set; } = 30;
    public int ResetFailedCountAfterMinutes { get; private set; } = 15;

    // ========================================
    // CONFIGURAÇÕES DE SESSÃO
    // ========================================

    public int SessionTimeoutMinutes { get; private set; } = 30;
    public int RefreshTokenExpirationDays { get; private set; } = 7;
    public bool RequireTwoFactorForAdmins { get; private set; }

    // ========================================
    // OUTRAS CONFIGURAÇÕES
    // ========================================

    public bool AllowConcurrentSessions { get; private set; } = true;
    public int MaxConcurrentSessions { get; private set; } = 3;

    // ========================================
    // AUDITORIA
    // ========================================

    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // ========================================
    // CONSTRUTORES
    // ========================================

    private SecurityPolicy() { } // EF Core

    public SecurityPolicy(string policyName, Guid? idSaaS = null)
    {
        Id = Guid.NewGuid();
        PolicyName = policyName;
        IdSaaS = idSaaS;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // ========================================
    // MÉTODOS DE NEGÓCIO
    // ========================================

    public void UpdatePasswordPolicy(
        int minLength,
        bool requireDigit,
        bool requireUppercase,
        bool requireLowercase,
        bool requireNonAlphanumeric,
        int? expirationDays,
        int historyCount)
    {
        PasswordMinLength = minLength;
        PasswordRequireDigit = requireDigit;
        PasswordRequireUppercase = requireUppercase;
        PasswordRequireLowercase = requireLowercase;
        PasswordRequireNonAlphanumeric = requireNonAlphanumeric;
        PasswordExpirationDays = expirationDays;
        PasswordHistoryCount = historyCount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLockoutPolicy(int maxAttempts, int lockoutDuration, int resetAfterMinutes)
    {
        MaxFailedAccessAttempts = maxAttempts;
        LockoutDurationMinutes = lockoutDuration;
        ResetFailedCountAfterMinutes = resetAfterMinutes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSessionPolicy(int sessionTimeout, int refreshTokenExpiration, bool require2FAForAdmins)
    {
        SessionTimeoutMinutes = sessionTimeout;
        RefreshTokenExpirationDays = refreshTokenExpiration;
        RequireTwoFactorForAdmins = require2FAForAdmins;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateConcurrentSessionsPolicy(bool allowConcurrent, int maxSessions)
    {
        AllowConcurrentSessions = allowConcurrent;
        MaxConcurrentSessions = maxSessions;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}