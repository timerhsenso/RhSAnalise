namespace RhSensoERP.Identity.Application.DTOs.Auth;

/// <summary>
/// Informações básicas do usuário autenticado.
/// </summary>
public sealed record UserInfoDto
{
    public Guid Id { get; init; }
    public string CdUsuario { get; init; } = string.Empty;
    public string DcUsuario { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? NoMatric { get; init; }
    public int? CdEmpresa { get; init; }
    public int? CdFilial { get; init; }
    public Guid? TenantId { get; init; }
    public bool TwoFactorEnabled { get; init; }
    public bool MustChangePassword { get; init; }
}