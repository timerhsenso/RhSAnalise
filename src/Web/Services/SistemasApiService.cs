// src/Web/Services/SistemasApiService.cs

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.Identity;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Implementação do serviço de comunicação com a API de Sistemas.
/// </summary>
public sealed class SistemasApiService : ISistemasApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SistemasApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SistemasApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<SistemasApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResultViewModel<SistemaViewModel>?> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        bool? ativo = null,
        CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ConfigurarAutenticacao();

            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            }

            if (ativo.HasValue)
            {
                queryParams.Add($"ativo={ativo.Value.ToString().ToLower()}");
            }

            var url = $"/api/identity/sistemas?{string.Join("&", queryParams)}";

            _logger.LogInformation("📤 [SISTEMAS] GET {Url}", url);

            var response = await _httpClient.GetAsync(url, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [SISTEMAS] Tempo: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ [SISTEMAS] Falha ao listar | Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<ApiResultViewModel<PagedResultViewModel<SistemaViewModel>>>(content, JsonOptions);

            if (result?.IsSuccess == true && result.Value != null)
            {
                _logger.LogInformation("✅ [SISTEMAS] Listagem OK | Total: {Count}", result.Value.TotalCount);
                return result.Value;
            }

            // Tenta deserializar diretamente se não estiver encapsulado
            var directResult = JsonSerializer.Deserialize<PagedResultViewModel<SistemaViewModel>>(content, JsonOptions);
            return directResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "💥 [SISTEMAS] Erro ao listar | Tempo: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<SistemaViewModel?> GetByIdAsync(string cdSistema, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ConfigurarAutenticacao();

            var url = $"/api/identity/sistemas/{Uri.EscapeDataString(cdSistema)}";

            _logger.LogInformation("📤 [SISTEMAS] GET {Url}", url);

            var response = await _httpClient.GetAsync(url, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [SISTEMAS] Tempo: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("❌ [SISTEMAS] Sistema não encontrado: {CdSistema}", cdSistema);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);
            var result = JsonSerializer.Deserialize<ApiResultViewModel<SistemaViewModel>>(content, JsonOptions);

            if (result?.IsSuccess == true && result.Value != null)
            {
                _logger.LogInformation("✅ [SISTEMAS] Sistema encontrado: {CdSistema}", cdSistema);
                return result.Value;
            }

            // Tenta deserializar diretamente
            var directResult = JsonSerializer.Deserialize<SistemaViewModel>(content, JsonOptions);
            return directResult;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "💥 [SISTEMAS] Erro ao buscar {CdSistema} | Tempo: {ElapsedMs}ms", cdSistema, stopwatch.ElapsedMilliseconds);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<ApiResultViewModel<string>?> CreateAsync(CreateSistemaViewModel model, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ConfigurarAutenticacao();

            var request = new
            {
                CdSistema = model.CdSistema,
                DcSistema = model.DcSistema,
                Ativo = model.Ativo
            };

            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("📤 [SISTEMAS] POST /api/identity/sistemas | Payload: {Json}", json);

            var response = await _httpClient.PostAsync("/api/identity/sistemas", content, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [SISTEMAS] Tempo: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ [SISTEMAS] Sistema criado: {CdSistema}", model.CdSistema);
                return new ApiResultViewModel<string> { IsSuccess = true, Value = model.CdSistema };
            }

            _logger.LogWarning("❌ [SISTEMAS] Falha ao criar | Status: {StatusCode} | Erro: {Error}",
                response.StatusCode, responseContent);

            var errorResult = JsonSerializer.Deserialize<ApiResultViewModel<string>>(responseContent, JsonOptions);
            return errorResult ?? new ApiResultViewModel<string>
            {
                IsSuccess = false,
                Error = new ApiErrorViewModel { Message = "Erro ao criar sistema." }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "💥 [SISTEMAS] Erro ao criar | Tempo: {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return new ApiResultViewModel<string>
            {
                IsSuccess = false,
                Error = new ApiErrorViewModel { Message = ex.Message }
            };
        }
    }

    /// <inheritdoc />
    public async Task<ApiResultViewModel<bool>?> UpdateAsync(string cdSistema, UpdateSistemaViewModel model, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ConfigurarAutenticacao();

            var request = new
            {
                DcSistema = model.DcSistema,
                Ativo = model.Ativo
            };

            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"/api/identity/sistemas/{Uri.EscapeDataString(cdSistema)}";

            _logger.LogInformation("📤 [SISTEMAS] PUT {Url} | Payload: {Json}", url, json);

            var response = await _httpClient.PutAsync(url, content, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [SISTEMAS] Tempo: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ [SISTEMAS] Sistema atualizado: {CdSistema}", cdSistema);
                return new ApiResultViewModel<bool> { IsSuccess = true, Value = true };
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("❌ [SISTEMAS] Falha ao atualizar | Status: {StatusCode} | Erro: {Error}",
                response.StatusCode, responseContent);

            var errorResult = JsonSerializer.Deserialize<ApiResultViewModel<bool>>(responseContent, JsonOptions);
            return errorResult ?? new ApiResultViewModel<bool>
            {
                IsSuccess = false,
                Error = new ApiErrorViewModel { Message = "Erro ao atualizar sistema." }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "💥 [SISTEMAS] Erro ao atualizar {CdSistema} | Tempo: {ElapsedMs}ms", cdSistema, stopwatch.ElapsedMilliseconds);
            return new ApiResultViewModel<bool>
            {
                IsSuccess = false,
                Error = new ApiErrorViewModel { Message = ex.Message }
            };
        }
    }

    /// <inheritdoc />
    public async Task<ApiResultViewModel<bool>?> DeleteAsync(string cdSistema, CancellationToken ct = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            ConfigurarAutenticacao();

            var url = $"/api/identity/sistemas/{Uri.EscapeDataString(cdSistema)}";

            _logger.LogInformation("📤 [SISTEMAS] DELETE {Url}", url);

            var response = await _httpClient.DeleteAsync(url, ct);

            stopwatch.Stop();
            _logger.LogInformation(
                "⏱️ [SISTEMAS] Tempo: {ElapsedMs}ms | Status: {StatusCode}",
                stopwatch.ElapsedMilliseconds,
                (int)response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("✅ [SISTEMAS] Sistema excluído: {CdSistema}", cdSistema);
                return new ApiResultViewModel<bool> { IsSuccess = true, Value = true };
            }

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("❌ [SISTEMAS] Falha ao excluir | Status: {StatusCode} | Erro: {Error}",
                response.StatusCode, responseContent);

            var errorResult = JsonSerializer.Deserialize<ApiResultViewModel<bool>>(responseContent, JsonOptions);
            return errorResult ?? new ApiResultViewModel<bool>
            {
                IsSuccess = false,
                Error = new ApiErrorViewModel { Message = "Erro ao excluir sistema." }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "💥 [SISTEMAS] Erro ao excluir {CdSistema} | Tempo: {ElapsedMs}ms", cdSistema, stopwatch.ElapsedMilliseconds);
            return new ApiResultViewModel<bool>
            {
                IsSuccess = false,
                Error = new ApiErrorViewModel { Message = ex.Message }
            };
        }
    }

    // ========================================
    // MÉTODOS PRIVADOS
    // ========================================

    private void ConfigurarAutenticacao()
    {
        var accessToken = _httpContextAccessor.HttpContext?.User.FindFirst("AccessToken")?.Value;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
