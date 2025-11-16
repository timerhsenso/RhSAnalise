using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Implementação do serviço de autenticação via API.
/// </summary>
public sealed class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthApiService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthApiResponse?> LoginAsync(LoginViewModel model, CancellationToken ct = default)
    {
        try
        {
            var loginRequest = new
            {
                cdUsuario = model.CdUsuario,
                senha = model.Senha,
                authStrategy = model.AuthStrategy
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/identity/auth/login", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning(
                    "Falha no login via API. Status: {StatusCode}, Erro: {Error}",
                    response.StatusCode,
                    errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var authResponse = JsonSerializer.Deserialize<AuthApiResponse>(responseContent, JsonOptions);

            _logger.LogInformation("Login via API bem-sucedido: {CdUsuario}", model.CdUsuario);

            return authResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login via API: {CdUsuario}", model.CdUsuario);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<AuthApiResponse?> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken ct = default)
    {
        try
        {
            var refreshRequest = new
            {
                accessToken,
                refreshToken
            };

            var content = new StringContent(
                JsonSerializer.Serialize(refreshRequest, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/identity/auth/refresh-token", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao renovar tokens. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<AuthApiResponse>(responseContent, JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao renovar tokens via API");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        try
        {
            var logoutRequest = new { refreshToken };

            var content = new StringContent(
                JsonSerializer.Serialize(logoutRequest, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/identity/auth/logout", content, ct);

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer logout via API");
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<UserInfoViewModel?> GetCurrentUserAsync(string accessToken, CancellationToken ct = default)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("/api/identity/auth/me", ct);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var userInfo = JsonSerializer.Deserialize<UserInfoViewModel>(content, JsonOptions);

            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados do usuário via API");
            return null;
        }
    }
}