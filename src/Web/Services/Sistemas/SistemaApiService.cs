// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sistemas;

/// <summary>
/// Implementação do serviço de API para Sistema.
/// </summary>
public class SistemaApiService : ISistemaApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SistemaApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = "api/identity/sistemas";

    public SistemaApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<SistemaApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region Private Methods

    private void SetAuthHeader()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {
            var token = context.User.FindFirst("AccessToken")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    private static ApiResponse<T> CreateErrorResponse<T>(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError { Message = message }
        };
    }

    private static ApiResponse<T> CreateSuccessResponse<T>(T? data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    private async Task<ApiResponse<T>> DeserializeResponseAsync<T>(
        HttpResponseMessage response,
        string operation)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                // Tenta deserializar como ApiResponse<T> primeiro
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                if (apiResponse != null)
                    return apiResponse;

                // Se não for ApiResponse, tenta deserializar direto como T
                var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return CreateSuccessResponse(data);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "[SISTEMA] Erro ao deserializar resposta de {Operation}", operation);
                return CreateErrorResponse<T>("Erro ao processar resposta do servidor");
            }
        }

        // Tenta extrair mensagem de erro
        string? errorMessage = null;
        try
        {
            var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
            errorMessage = errorResponse?.Message;
        }
        catch { }

        _logger.LogWarning("[SISTEMA] Erro em {Operation}: {Status} - {Content}",
            operation, response.StatusCode, content);

        return CreateErrorResponse<T>(errorMessage ?? $"Erro: {response.StatusCode}");
    }

    /// <summary>
    /// Deserializa resposta que retorna apenas o ID (string) da entidade criada.
    /// A API retorna Result<string> para Create, não Result<SistemaDto>.
    /// </summary>
    private async Task<ApiResponse<string>> DeserializeCreateResponseAsync(
        HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                // A API retorna Result<string> com o ID criado
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // Verifica se é Result<T> (tem isSuccess e value)
                if (root.TryGetProperty("isSuccess", out var isSuccessProp) && isSuccessProp.GetBoolean())
                {
                    if (root.TryGetProperty("value", out var valueProp))
                    {
                        var id = valueProp.GetString() ?? string.Empty;
                        return CreateSuccessResponse(id);
                    }
                }

                // Se tem error
                if (root.TryGetProperty("error", out var errorProp) &&
                    errorProp.TryGetProperty("message", out var msgProp))
                {
                    return CreateErrorResponse<string>(msgProp.GetString() ?? "Erro desconhecido");
                }

                return CreateSuccessResponse(string.Empty);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "[SISTEMA] Erro ao deserializar resposta de Create");
                return CreateErrorResponse<string>("Erro ao processar resposta do servidor");
            }
        }

        // Erro - tenta extrair mensagem
        string? errorMessage = null;
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errorProp) &&
                errorProp.TryGetProperty("message", out var msgProp))
            {
                errorMessage = msgProp.GetString();
            }
        }
        catch { }

        _logger.LogWarning("[SISTEMA] Erro em Create: {Status} - {Content}",
            response.StatusCode, content);

        return CreateErrorResponse<string>(errorMessage ?? $"Erro: {response.StatusCode}");
    }

    #endregion

    #region IApiService Implementation

    /// <inheritdoc/>
    public async Task<ApiResponse<PagedResult<SistemaDto>>> GetPagedAsync(
        int page, int pageSize, string? search = null)
    {
        try
        {
            SetAuthHeader();

            var queryParams = $"?page={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParams += $"&search={Uri.EscapeDataString(search)}";
            }

            var response = await _httpClient.GetAsync($"{ApiRoute}{queryParams}");
            return await DeserializeResponseAsync<PagedResult<SistemaDto>>(response, "GetPaged");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em GetPagedAsync");
            return CreateErrorResponse<PagedResult<SistemaDto>>("Erro de conexão com o servidor");
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<IEnumerable<SistemaDto>>> GetAllAsync()
    {
        try
        {
            SetAuthHeader();

            var result = await GetPagedAsync(1, 10000, null);

            if (!result.Success || result.Data == null)
            {
                return CreateErrorResponse<IEnumerable<SistemaDto>>(result.Message ?? "Erro ao buscar dados");
            }

            return CreateSuccessResponse<IEnumerable<SistemaDto>>(result.Data.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em GetAllAsync");
            return CreateErrorResponse<IEnumerable<SistemaDto>>("Erro de conexão com o servidor");
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<SistemaDto>> GetByIdAsync(string id)
    {
        try
        {
            SetAuthHeader();

            var response = await _httpClient.GetAsync($"{ApiRoute}/{id}");
            return await DeserializeResponseAsync<SistemaDto>(response, "GetById");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em GetByIdAsync");
            return CreateErrorResponse<SistemaDto>("Erro de conexão com o servidor");
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<SistemaDto>> CreateAsync(CreateSistemaDto dto)
    {
        try
        {
            SetAuthHeader();

            // Log do DTO antes de enviar
            _logger.LogDebug("[SISTEMA] CreateAsync - DTO recebido: CdSistema={CdSistema}, DcSistema={DcSistema}, Ativo={Ativo}",
                dto.CdSistema, dto.DcSistema, dto.Ativo);

            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            _logger.LogDebug("[SISTEMA] CreateAsync - JSON a enviar: {Json}", json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiRoute, content);

            // A API retorna Result<string> (o ID criado), não Result<SistemaDto>
            var createResult = await DeserializeCreateResponseAsync(response);

            if (!createResult.Success)
            {
                return CreateErrorResponse<SistemaDto>(createResult.Message ?? "Erro ao criar registro");
            }

            // Retorna um DTO com o ID criado
            var createdDto = new SistemaDto
            {
                CdSistema = createResult.Data ?? dto.CdSistema,
                DcSistema = dto.DcSistema,
                Ativo = dto.Ativo
            };

            return CreateSuccessResponse(createdDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em CreateAsync");
            return CreateErrorResponse<SistemaDto>("Erro de conexão com o servidor");
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<SistemaDto>> UpdateAsync(string id, UpdateSistemaDto dto)
    {
        try
        {
            SetAuthHeader();

            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{ApiRoute}/{id}", content);

            // Update também pode retornar Result<string> ou Result<SistemaDto>
            // Vamos tratar de forma similar ao Create
            if (response.IsSuccessStatusCode)
            {
                var updatedDto = new SistemaDto
                {
                    CdSistema = id,
                    DcSistema = dto.DcSistema,
                    Ativo = dto.Ativo
                };
                return CreateSuccessResponse(updatedDto);
            }

            return await DeserializeResponseAsync<SistemaDto>(response, "Update");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em UpdateAsync");
            return CreateErrorResponse<SistemaDto>("Erro de conexão com o servidor");
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            SetAuthHeader();

            var response = await _httpClient.DeleteAsync($"{ApiRoute}/{id}");

            if (response.IsSuccessStatusCode)
            {
                return CreateSuccessResponse(true);
            }

            return await DeserializeResponseAsync<bool>(response, "Delete");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em DeleteAsync");
            return CreateErrorResponse<bool>("Erro de conexão com o servidor");
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<string> ids)
    {
        try
        {
            var result = await DeleteBatchAsync(ids);

            return new ApiResponse<bool>
            {
                Success = result.Success,
                Data = result.Success && (result.Data?.AllSucceeded ?? false),
                Error = result.Error
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em DeleteMultipleAsync");
            return CreateErrorResponse<bool>("Erro de conexão com o servidor");
        }
    }

    #endregion

    #region IBatchDeleteService Implementation

    /// <inheritdoc/>
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<string> ids)
    {
        try
        {
            SetAuthHeader();

            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{ApiRoute}/batch")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            return await DeserializeResponseAsync<BatchDeleteResultDto>(response, "DeleteBatch");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMA] Exceção em DeleteBatchAsync");
            return CreateErrorResponse<BatchDeleteResultDto>("Erro de conexão com o servidor");
        }
    }

    #endregion
}