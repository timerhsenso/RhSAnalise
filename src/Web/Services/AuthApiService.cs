using System.Diagnostics;
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

    // ✅ FIX: Removido camelCase - API usa PascalCase
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AuthApiResponse?> LoginAsync(LoginViewModel model, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "🔐 [LOGIN] Iniciando autenticação para usuário: {CdUsuario}",
                model.CdUsuario);

            // ✅ FIX: Removido AuthStrategy - API determina automaticamente
            var loginRequest = new
            {
                LoginIdentifier = model.CdUsuario,
                Senha = model.Senha,
                RememberMe = model.RememberMe
            };

            var jsonPayload = JsonSerializer.Serialize(loginRequest, JsonOptions);
            _logger.LogDebug("📤 [LOGIN] Payload JSON: {Json}", jsonPayload);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var requestUrl = $"{_httpClient.BaseAddress}/api/identity/auth/login";
            _logger.LogInformation("📤 [LOGIN] Enviando requisição para: {Url}", requestUrl);

            var requestStopwatch = Stopwatch.StartNew();
            var response = await _httpClient.PostAsync("/api/identity/auth/login", content, ct);
            requestStopwatch.Stop();

            _logger.LogInformation(
                "⏱️ [LOGIN] Tempo de resposta da API: {ElapsedMs}ms | Status: {StatusCode}",
                requestStopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning(
                    "❌ [LOGIN] Falha na autenticação | Status: {StatusCode} | Erro: {Error}",
                    response.StatusCode,
                    errorContent);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var authResponse = JsonSerializer.Deserialize<AuthApiResponse>(responseContent, JsonOptions);

            stopwatch.Stop();
            _logger.LogInformation(
                "✅ [LOGIN] Autenticação bem-sucedida | Usuário: {CdUsuario} | Tempo total: {ElapsedMs}ms",
                model.CdUsuario,
                stopwatch.ElapsedMilliseconds);

            return authResponse;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            stopwatch.Stop();
            _logger.LogError(
                "⏰ [LOGIN] TIMEOUT: A API não respondeu a tempo | Usuário: {CdUsuario} | Tempo decorrido: {ElapsedMs}ms | Timeout configurado: {TimeoutSeconds}s",
                model.CdUsuario,
                stopwatch.ElapsedMilliseconds,
                _httpClient.Timeout.TotalSeconds);

            _logger.LogError(
                "💡 [LOGIN] DICA: Verifique se a API está rodando e se o banco de dados está acessível");

            return null;
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "🌐 [LOGIN] Erro de conexão HTTP | Usuário: {CdUsuario} | Tempo decorrido: {ElapsedMs}ms",
                model.CdUsuario,
                stopwatch.ElapsedMilliseconds);

            _logger.LogError(
                "💡 [LOGIN] DICA: Verifique se a URL da API está correta: {BaseUrl}",
                _httpClient.BaseAddress);

            return null;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "💥 [LOGIN] Erro inesperado | Usuário: {CdUsuario} | Tempo decorrido: {ElapsedMs}ms",
                model.CdUsuario,
                stopwatch.ElapsedMilliseconds);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<AuthApiResponse?> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("🔄 [REFRESH] Iniciando renovação de tokens");

            // ✅ FIX: PascalCase
            var refreshRequest = new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var content = new StringContent(
                JsonSerializer.Serialize(refreshRequest, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/identity/auth/refresh-token", content, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [REFRESH] Tempo de resposta: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "❌ [REFRESH] Falha ao renovar tokens | Status: {StatusCode}",
                    response.StatusCode);
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var authResponse = JsonSerializer.Deserialize<AuthApiResponse>(responseContent, JsonOptions);

            _logger.LogInformation("✅ [REFRESH] Tokens renovados com sucesso");

            return authResponse;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "💥 [REFRESH] Erro ao renovar tokens | Tempo decorrido: {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("🚪 [LOGOUT] Iniciando logout");

            // ✅ FIX: PascalCase
            var logoutRequest = new { RefreshToken = refreshToken };

            var content = new StringContent(
                JsonSerializer.Serialize(logoutRequest, JsonOptions),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/identity/auth/logout", content, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [LOGOUT] Tempo de resposta: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ [LOGOUT] Logout realizado com sucesso");
            }
            else
            {
                _logger.LogWarning("⚠️ [LOGOUT] Logout retornou status: {StatusCode}", response.StatusCode);
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "💥 [LOGOUT] Erro ao fazer logout | Tempo decorrido: {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<UserInfoViewModel?> GetCurrentUserAsync(string accessToken, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("👤 [USER-INFO] Obtendo informações do usuário");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync("/api/identity/auth/me", ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [USER-INFO] Tempo de resposta: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ [USER-INFO] Falha ao obter informações | Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var userInfo = JsonSerializer.Deserialize<UserInfoViewModel>(content, JsonOptions);

            _logger.LogInformation("✅ [USER-INFO] Informações obtidas com sucesso");

            return userInfo;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "💥 [USER-INFO] Erro ao obter dados do usuário | Tempo decorrido: {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<UserPermissionsViewModel?> GetUserPermissionsAsync(
        string cdUsuario,
        string? cdSistema = null,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "🔑 [PERMISSIONS] Obtendo permissões | Usuário: {CdUsuario} | Sistema: {CdSistema}",
                cdUsuario,
                cdSistema ?? "Todos");

            var url = $"/api/identity/permissoes/{cdUsuario}";
            if (!string.IsNullOrWhiteSpace(cdSistema))
            {
                url += $"?cdSistema={cdSistema}";
            }

            var response = await _httpClient.GetAsync(url, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [PERMISSIONS] Tempo de resposta: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "❌ [PERMISSIONS] Falha ao obter permissões | Status: {StatusCode}",
                    response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var permissions = JsonSerializer.Deserialize<UserPermissionsViewModel>(content, JsonOptions);

            _logger.LogInformation(
                "✅ [PERMISSIONS] Permissões obtidas | Grupos: {GruposCount} | Funções: {FuncoesCount} | Botões: {BotoesCount}",
                permissions?.Grupos?.Count ?? 0,
                permissions?.Funcoes?.Count ?? 0,
                permissions?.Botoes?.Count ?? 0);

            return permissions;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "💥 [PERMISSIONS] Erro ao obter permissões | Usuário: {CdUsuario} | Tempo decorrido: {ElapsedMs}ms",
                cdUsuario,
                stopwatch.ElapsedMilliseconds);
            return null;
        }
    }
}