using System;
using System.Collections.Generic;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Identity.Domain.Entities;

/// <summary>
/// Entidade de usuários do sistema (tabela dbo.tuse1).
/// Mapeada fielmente aos campos do SQL: PK em cdusuario (legado), Id (GUID) como chave única,
/// campos de credenciais, status, auditoria, 2FA e multi-tenant.
/// </summary>
public class Usuario : BaseEntity
{
    // ========= PK (legado) / Identidade =========
    /// <summary>PK lógica (varchar(30)) — coluna: cdusuario (NOT NULL).</summary>
    public string CdUsuario { get; set; } = string.Empty;

    /// <summary>Nome do usuário (varchar(50)) — coluna: dcusuario (NOT NULL).</summary>
    public string DcUsuario { get; set; } = string.Empty;

    /// <summary>GUID único alternativo (uniqueidentifier) — coluna: id (UNIQUE NOT NULL). Usado pelo BaseEntity.Id.</summary>
    /// <remarks>O <see cref="BaseEntity.Id"/> é mapeado para a coluna "id" via Fluent API.</remarks>

    // ========= Credenciais / Normalização =========
    /// <summary>Senha (legado) (nvarchar(20)) — coluna: senhauser (NULL).</summary>
    public string? SenhaUser { get; set; }

    /// <summary>Hash de senha (varchar(255)) — coluna: PasswordHash (NULL).</summary>
    public string? PasswordHash { get; set; }

    /// <summary>Salt da senha (varchar(255)) — coluna: PasswordSalt (NULL).</summary>
    public string? PasswordSalt { get; set; }

    /// <summary>Nome de usuário normalizado (varchar(30)) — coluna: normalizedusername (NULL).</summary>
    public string? NormalizedUserName { get; set; }

    // ========= Dados funcionais / empresa =========
    /// <summary>Impressora/checagem (varchar(50)) — coluna: nmimpcche (NULL).</summary>
    public string? NmImpcche { get; set; }

    /// <summary>Tipo de usuário (char(1)) — coluna: tpusuario (NOT NULL).</summary>
    public char TpUsuario { get; set; }

    /// <summary>Matrícula (char(8)) — coluna: nomatric (NULL).</summary>
    public string? NoMatric { get; set; }

    /// <summary>Código da empresa (int) — coluna: cdempresa (NULL).</summary>
    public int? CdEmpresa { get; set; }

    /// <summary>Código da filial (int) — coluna: cdfilial (NULL).</summary>
    public int? CdFilial { get; set; }

    /// <summary>Número do usuário (int) — coluna: nouser (NOT NULL).</summary>
    public int NoUser { get; set; }

    /// <summary>E-mail (varchar(100)) — coluna: email_usuario (NULL).</summary>
    public string? Email_Usuario { get; set; }

    /// <summary>Flag de ativo (char(1)) — coluna: flativo (NOT NULL).</summary>
    public char FlAtivo { get; set; }

    /// <summary>Recusa de e-mail (char(1)) — coluna: flnaorecebeemail (NULL).</summary>
    public char? FlNaoRecebeEmail { get; set; }

    /// <summary>ID do funcionário (uniqueidentifier) — coluna: idfuncionario (NULL).</summary>
    public Guid? IdFuncionario { get; set; }

    // ========= Multi-tenant =========
    /// <summary>Tenant principal (uniqueidentifier) — coluna: TenantPrincipal (NULL).</summary>
    public Guid? TenantPrincipal { get; set; }

    /// <summary>TenantId (uniqueidentifier) — coluna: TenantId (NULL).</summary>
    public Guid? TenantId { get; set; }

    // ========= Modo de autenticação / confirmação e-mail =========
    /// <summary>Modo de autenticação (varchar(20)) — coluna: AuthMode (NOT NULL, default 'OnPrem').</summary>
    public string AuthMode { get; set; } = "OnPrem";

    /// <summary>E-mail confirmado (bit) — coluna: EmailConfirmed (NOT NULL, default 0).</summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>Token de confirmação de e-mail (varchar(255)) — coluna: EmailConfirmationToken (NULL).</summary>
    public string? EmailConfirmationToken { get; set; }

    /// <summary>Expiração do token de confirmação (datetime2) — coluna: EmailConfirmationTokenExpiry (NULL).</summary>
    public DateTime? EmailConfirmationTokenExpiry { get; set; }

    // ========= Reset de senha =========
    /// <summary>Token de reset de senha (varchar(255)) — coluna: PasswordResetToken (NULL).</summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>Expiração do token de reset (datetime2) — coluna: PasswordResetTokenExpiry (NULL).</summary>
    public DateTime? PasswordResetTokenExpiry { get; set; }

    /// <summary>Data/hora da solicitação de reset (datetime2) — coluna: PasswordResetRequestedAt (NULL).</summary>
    public DateTime? PasswordResetRequestedAt { get; set; }

    /// <summary>Quem solicitou o reset (varchar(30)) — coluna: PasswordResetRequestedBy (NULL).</summary>
    public string? PasswordResetRequestedBy { get; set; }

    /// <summary>Quantidade de tentativas de login (int) — coluna: LoginAttempts (NOT NULL, default 0).</summary>
    public int LoginAttempts { get; set; }

    /// <summary>Bloqueado até (datetime2) — coluna: LockedUntil (NULL).</summary>
    public DateTime? LockedUntil { get; set; }

    /// <summary>Última falha de login (datetime2) — coluna: LastFailedLoginAt (NULL).</summary>
    public DateTime? LastFailedLoginAt { get; set; }

    /// <summary>Último login (datetime2) — coluna: LastLoginAt (NULL).</summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Última troca de senha (datetime2) — coluna: LastPasswordChangedAt (NULL).</summary>
    public DateTime? LastPasswordChangedAt { get; set; }

    // ========= 2FA =========
    /// <summary>2FA habilitado (bit) — coluna: TwoFactorEnabled (NOT NULL, default 0).</summary>
    public bool TwoFactorEnabled { get; set; }

    /// <summary>Segredo 2FA (varchar(255)) — coluna: TwoFactorSecret (NULL).</summary>
    public string? TwoFactorSecret { get; set; }

    /// <summary>Códigos de backup 2FA (nvarchar(max)) — coluna: TwoFactorBackupCodes (NULL).</summary>
    public string? TwoFactorBackupCodes { get; set; }

    // ========= Auditoria / rede =========
    /// <summary>Criado em (datetime2) — coluna: CreatedAt (NOT NULL, default getutcdate()).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Atualizado em (datetime2) — coluna: UpdatedAt (NOT NULL, default getutcdate()).</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Criado por (varchar(30)) — coluna: CreatedBy (NULL).</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Atualizado por (varchar(30)) — coluna: UpdatedBy (NULL).</summary>
    public string? UpdatedBy { get; set; }

    /// <summary>Último User-Agent (varchar(500)) — coluna: LastUserAgent (NULL).</summary>
    public string? LastUserAgent { get; set; }

    /// <summary>Último IP (varchar(45)) — coluna: LastIpAddress (NULL).</summary>
    public string? LastIpAddress { get; set; }

    // ========= Relacionamentos =========
    /// <summary>Associações usuário-grupo (tabela usrh1).</summary>
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
