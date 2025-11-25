// =============================================================================
// RHSENSOERP WEB - BANCO API SERVICE
// =============================================================================
// Arquivo: src/Web/Services/Bancos/BancoApiService.cs
// Descrição: Serviço para comunicação com a API de Bancos
// Versão: 3.0 (Corrigido - Usa IHttpClientFactory)
// =============================================================================

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Models.Bancos;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Bancos;

/// <summary>
/// Serviço de comunicação com a API de Bancos.
/// </summary>
public sealed class BancoApiService : IBancoApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BancoApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string BaseEndpoint = "/api/v1/gestaodepessoas/tabelas/pessoal/bancos";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public BancoApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<BancoApiService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var token = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                "access_token");
            
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.User.FindFirst("AccessToken")?.Value;
            }
            
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
        
        return client;
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<PagedResult<BancoDto>>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            }

            var url = $"{BaseEndpoint}?{string.Join("&", queryParams)}";
            
            _logger.LogDebug("📤 [BANCOS] GET {Url}", url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("📥 [BANCOS] Status: {Status}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<PagedResult<BancoDto>>>(content, JsonOptions);
                return result ?? new ApiResponse<PagedResult<BancoDto>> 
                { 
                    Success = false, 
                    Error = new ApiError { Message = "Erro ao deserializar resposta" } 
                };
            }

            _logger.LogWarning("❌ [BANCOS] Erro ao buscar dados: {Status}", response.StatusCode);
            return new ApiResponse<PagedResult<BancoDto>> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Erro ao buscar dados" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao buscar dados paginados");
            return new ApiResponse<PagedResult<BancoDto>> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<IEnumerable<BancoDto>>> GetAllAsync()
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();
            var response = await client.GetAsync($"{BaseEndpoint}/all");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<IEnumerable<BancoDto>>>(content, JsonOptions);
                return result ?? new ApiResponse<IEnumerable<BancoDto>> 
                { 
                    Success = false, 
                    Error = new ApiError { Message = "Erro ao deserializar resposta" } 
                };
            }

            return new ApiResponse<IEnumerable<BancoDto>> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Erro ao buscar registros" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao buscar todos os registros");
            return new ApiResponse<IEnumerable<BancoDto>> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<BancoDto>> GetByIdAsync(string id)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();
            var response = await client.GetAsync($"{BaseEndpoint}/{Uri.EscapeDataString(id)}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<BancoDto>>(content, JsonOptions);
                return result ?? new ApiResponse<BancoDto> 
                { 
                    Success = false, 
                    Error = new ApiError { Message = "Erro ao deserializar resposta" } 
                };
            }

            return new ApiResponse<BancoDto> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Registro não encontrado" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao buscar registro {Id}", id);
            return new ApiResponse<BancoDto> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<BancoDto>> CreateAsync(CreateBancoDto dto)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var json = JsonSerializer.Serialize(dto, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("📤 [BANCOS] POST {Endpoint} | Payload: {Json}", BaseEndpoint, json);

            var response = await client.PostAsync(BaseEndpoint, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<BancoDto>>(content, JsonOptions);
                return result ?? new ApiResponse<BancoDto> 
                { 
                    Success = true, 
                    Error = new ApiError { Message = "Registro criado com sucesso" } 
                };
            }

            _logger.LogWarning("❌ [BANCOS] Erro ao criar: {Status} | {Content}", 
                response.StatusCode, content);

            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<BancoDto>>(content, JsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<BancoDto> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Erro ao criar registro" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao criar registro");
            return new ApiResponse<BancoDto> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<BancoDto>> UpdateAsync(string id, UpdateBancoDto dto)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var json = JsonSerializer.Serialize(dto, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogDebug("📤 [BANCOS] PUT {Endpoint}/{Id} | Payload: {Json}", 
                BaseEndpoint, id, json);

            var response = await client.PutAsync($"{BaseEndpoint}/{Uri.EscapeDataString(id)}", httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ApiResponse<BancoDto>>(content, JsonOptions);
                return result ?? new ApiResponse<BancoDto> 
                { 
                    Success = true, 
                    Error = new ApiError { Message = "Registro atualizado com sucesso" } 
                };
            }

            _logger.LogWarning("❌ [BANCOS] Erro ao atualizar: {Status} | {Content}", 
                response.StatusCode, content);

            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<BancoDto>>(content, JsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return new ApiResponse<BancoDto> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Erro ao atualizar registro" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao atualizar registro {Id}", id);
            return new ApiResponse<BancoDto> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            _logger.LogDebug("📤 [BANCOS] DELETE {Endpoint}/{Id}", BaseEndpoint, id);

            var response = await client.DeleteAsync($"{BaseEndpoint}/{Uri.EscapeDataString(id)}");

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool> 
                { 
                    Success = true, 
                    Data = true, 
                    Error = new ApiError { Message = "Registro excluído com sucesso" } 
                };
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("❌ [BANCOS] Erro ao excluir: {Status} | {Content}", 
                response.StatusCode, content);

            return new ApiResponse<bool> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Erro ao excluir registro" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao excluir registro {Id}", id);
            return new ApiResponse<bool> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<string> ids)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var json = JsonSerializer.Serialize(ids, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseEndpoint}/multiple")
            {
                Content = httpContent
            };

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool> 
                { 
                    Success = true, 
                    Data = true, 
                    Error = new ApiError { Message = "Registros excluídos com sucesso" } 
                };
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("❌ [BANCOS] Erro ao excluir múltiplos: {Status} | {Content}", 
                response.StatusCode, content);

            return new ApiResponse<bool> 
            { 
                Success = false, 
                Error = new ApiError { Message = "Erro ao excluir registros" } 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 [BANCOS] Erro ao excluir múltiplos registros");
            return new ApiResponse<bool> 
            { 
                Success = false, 
                Error = new ApiError { Message = ex.Message } 
            };
        }
    }
}
