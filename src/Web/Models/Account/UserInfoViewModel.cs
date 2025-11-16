namespace RhSensoERP.Web.Models.Account;

/// <summary>
/// ViewModel com informações do usuário autenticado.
/// </summary>
public sealed class UserInfoViewModel
{
    public string CdUsuario { get; set; } = string.Empty;
    public string NmUsuario { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NmFuncionario { get; set; }
    public bool IsActive { get; set; }
}