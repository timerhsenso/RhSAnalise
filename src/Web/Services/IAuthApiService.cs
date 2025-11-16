using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Serviço para comunicação com a API de autenticação.
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Realiza login na API.
    /// </summary>
    Task<AuthApiResponse?> LoginAsync(LoginViewModel model, CancellationToken ct = default);

    /// <summary>
    /// Renova tokens usando refresh token.
    /// </summary>
    Task<AuthApiResponse?> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Realiza logout na API.
    /// </summary>
    Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Obtém informações do usuário autenticado.
    /// </summary>
    Task<UserInfoViewModel?> GetCurrentUserAsync(string accessToken, CancellationToken ct = default);
}

/// <summary>
/// Resposta da API de autenticação.
/// </summary>
public sealed class AuthApiResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoData? User { get; set; }
}

/// <summary>
/// Dados do usuário na resposta de login.
/// </summary>
public sealed class UserInfoData
{
    public string CdUsuario { get; set; } = string.Empty;
    public string NmUsuario { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NmFuncionario { get; set; }
}