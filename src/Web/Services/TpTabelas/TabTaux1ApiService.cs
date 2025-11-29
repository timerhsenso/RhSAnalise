// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.5
// Entity: TabTaux1
// Data: 2025-11-28 23:45:52
// CORREÇÃO v2.5: Usando GetTokenAsync para obter JWT do AuthenticationTokens
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using RhSensoERP.Web.Models.TpTabelas;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TpTabelas;

/// <summary>
/// Implementação do serviço de API para Tipo de Tabela.
/// Consome a API backend gerada pelo Source Generator.
/// </summary>
public class TabTaux1ApiService : ITabTaux1ApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TabTaux1ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = "api/gestaodepessoas/tptabelas";

    public TabTaux1ApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<TabTaux1ApiService> logger)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Private Helpers

    /// <summary>
    /// Configura header de autenticação com token JWT.
    /// ✅ CORREÇÃO v2.5: Token está em AuthenticationTokens (StoreTokens no AccountController)
    /// </summary>
    private async Task SetAuthHeaderAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            // ✅ Token está em AuthenticationTokens, não em Claims
            var token = await context.GetTokenAsync("access_token");
            
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("🔑 [TABTAUX1] Token JWT configurado para requisição");
            }
            else
            {
                _logger.LogWarning("⚠️ [TABTAUX1] Token JWT não encontrado nos AuthenticationTokens");
            }
        }
        else
        {
            _logger.LogWarning("⚠️ [TABTAUX1] Usuário não autenticado");
        }
    }

    /// <summary>
    /// Cria ApiResponse de sucesso.
    /// </summary>
    private static ApiResponse<T> Success<T>(T? data) => new()
    {
        Success = true,
        Data = data
    };

    /// <summary>
    /// Cria ApiResponse de erro.
    /// NOTA: Message é computed (=> Error?.Message), então usamos Error.
    /// </summary>
    private static ApiResponse<T> Fail<T>(string message) => new()
    {
        Success = false,
        Error = new ApiError { Message = message }
    };

    /// <summary>
    /// Processa resposta HTTP do backend.
    /// </summary>
    private async Task<ApiResponse<T>> ProcessResponseAsync<T>(
        HttpResponseMessage response, 
        string operation)
    {
        var content = await response.Content.ReadAsStringAsync();
        
        _logger.LogDebug("[TABTAUX1] {Op} - Status: {Status}, Content: {Content}", 
            operation, response.StatusCode, content.Length > 500 ? content[..500] : content);

        try
        {
            if (response.IsSuccessStatusCode)
            {
                // Tenta deserializar estrutura Result<T> do backend
                var backendResult = JsonSerializer.Deserialize<BackendResult<T>>(content, _jsonOptions);
                if (backendResult?.IsSuccess == true)
                {
                    return Success(backendResult.Value ?? backendResult.Data);
                }
                
                var errorMsg = backendResult?.Error?.Message ?? "Erro desconhecido";
                _logger.LogWarning("[TABTAUX1] Erro em {Op}: {Err}", operation, errorMsg);
                return Fail<T>(errorMsg);
            }

            // Erro HTTP - tenta extrair mensagem
            try
            {
                var errorResult = JsonSerializer.Deserialize<BackendResult<object>>(content, _jsonOptions);
                var msg = errorResult?.Error?.Message ?? $"Erro HTTP {(int)response.StatusCode}";
                _logger.LogWarning("[TABTAUX1] HTTP {Status} em {Op}: {Msg}", 
                    response.StatusCode, operation, msg);
                return Fail<T>(msg);
            }
            catch
            {
                return Fail<T>($"Erro HTTP {(int)response.StatusCode}: {response.ReasonPhrase}");
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro JSON em {Op}", operation);
            return Fail<T>("Erro ao processar resposta do servidor");
        }
    }

    #endregion

    #region IApiService Implementation

    public async Task<ApiResponse<PagedResult<TabTaux1Dto>>> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var query = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                query += $"&search={Uri.EscapeDataString(search)}";

            _logger.LogDebug("[TABTAUX1] GET {Route}{Query}", ApiRoute, query);

            var response = await _httpClient.GetAsync($"{ApiRoute}{query}");
            return await ProcessResponseAsync<PagedResult<TabTaux1Dto>>(response, "GetPaged");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro de conexão em GetPagedAsync");
            return Fail<PagedResult<TabTaux1Dto>>("Erro de conexão com o servidor");
        }
        catch (TaskCanceledException)
        {
            return Fail<PagedResult<TabTaux1Dto>>("Tempo limite excedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em GetPagedAsync");
            return Fail<PagedResult<TabTaux1Dto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<TabTaux1Dto>>> GetAllAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}?page=1&pageSize=10000");
            var result = await ProcessResponseAsync<PagedResult<TabTaux1Dto>>(response, "GetAll");
            
            if (result.Success && result.Data != null)
                return Success<IEnumerable<TabTaux1Dto>>(result.Data.Items);
            
            return Fail<IEnumerable<TabTaux1Dto>>(result.Error?.Message ?? "Erro ao buscar dados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em GetAllAsync");
            return Fail<IEnumerable<TabTaux1Dto>>(ex.Message);
        }
    }

    public async Task<ApiResponse<TabTaux1Dto>> GetByIdAsync(string id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"{ApiRoute}/{id}");
            return await ProcessResponseAsync<TabTaux1Dto>(response, "GetById");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro de conexão em GetByIdAsync");
            return Fail<TabTaux1Dto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em GetByIdAsync");
            return Fail<TabTaux1Dto>(ex.Message);
        }
    }

    public async Task<ApiResponse<TabTaux1Dto>> CreateAsync(CreateTabTaux1Request request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[TABTAUX1] POST {Route} - Body: {Body}", ApiRoute, json);
            
            var response = await _httpClient.PostAsync(ApiRoute, content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<TabTaux1Dto>(response, "Create");

            // Backend retorna Result<string> com o ID criado
            var responseJson = await response.Content.ReadAsStringAsync();
            var createResult = JsonSerializer.Deserialize<BackendResult<string>>(responseJson, _jsonOptions);
            
            if (createResult?.IsSuccess == true)
            {
                var createdId = createResult.Value;
                if (createdId == null && createResult.Data != null)
                    createdId = createResult.Data;
                    
                if (createdId != default)
                    return await GetByIdAsync(createdId!);
            }

            return Fail<TabTaux1Dto>(createResult?.Error?.Message ?? "Erro ao criar registro");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro de conexão em CreateAsync");
            return Fail<TabTaux1Dto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em CreateAsync");
            return Fail<TabTaux1Dto>(ex.Message);
        }
    }

    public async Task<ApiResponse<TabTaux1Dto>> UpdateAsync(string id, UpdateTabTaux1Request request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            _logger.LogDebug("[TABTAUX1] PUT {Route}/{Id} - Body: {Body}", ApiRoute, id, json);
            
            var response = await _httpClient.PutAsync($"{ApiRoute}/{id}", content);
            
            if (!response.IsSuccessStatusCode)
                return await ProcessResponseAsync<TabTaux1Dto>(response, "Update");

            return await GetByIdAsync(id);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro de conexão em UpdateAsync");
            return Fail<TabTaux1Dto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em UpdateAsync");
            return Fail<TabTaux1Dto>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{ApiRoute}/{id}");
            
            if (response.IsSuccessStatusCode)
                return Success(true);
            
            return await ProcessResponseAsync<bool>(response, "Delete");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro de conexão em DeleteAsync");
            return Fail<bool>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em DeleteAsync");
            return Fail<bool>(ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<string> ids)
    {
        try
        {
            var result = await DeleteBatchAsync(ids);
            
            if (result.Success && result.Data != null)
                return Success(result.Data.FailureCount == 0);
            
            return Fail<bool>(result.Error?.Message ?? "Erro ao excluir registros");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em DeleteMultipleAsync");
            return Fail<bool>(ex.Message);
        }
    }

    #endregion

    #region IBatchDeleteService Implementation

    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<string> ids)
    {
        try
        {
            await SetAuthHeaderAsync();
            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            _logger.LogDebug("[TABTAUX1] DELETE {Route}/batch - Body: {Body}", ApiRoute, json);
            
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiRoute}/batch")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var backendResult = JsonSerializer.Deserialize<BackendResult<BackendBatchDeleteResult>>(content, _jsonOptions);
            
            if (backendResult?.IsSuccess == true)
            {
                var data = backendResult.Value ?? backendResult.Data;
                if (data != null)
                {
                    var dto = new BatchDeleteResultDto
                    {
                        SuccessCount = data.SuccessCount,
                        FailureCount = data.FailureCount,
                        Errors = data.Errors?.Select(e => new BatchDeleteErrorDto
                        {
                            Code = e.Id ?? e.Code ?? string.Empty,
                            Message = e.Message ?? string.Empty
                        }).ToList() ?? new List<BatchDeleteErrorDto>()
                    };
                    
                    return Success(dto);
                }
            }
            
            return Fail<BatchDeleteResultDto>(backendResult?.Error?.Message ?? "Erro ao excluir em lote");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Erro de conexão em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>("Erro de conexão com o servidor");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TABTAUX1] Exceção em DeleteBatchAsync");
            return Fail<BatchDeleteResultDto>(ex.Message);
        }
    }

    #endregion

    #region Backend DTOs

    private sealed class BackendResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public T? Data { get; set; }
        public BackendError? Error { get; set; }
    }

    private sealed class BackendError
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    private sealed class BackendBatchDeleteResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<BackendBatchDeleteError>? Errors { get; set; }
    }

    private sealed class BackendBatchDeleteError
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Message { get; set; }
    }

    #endregion
}
