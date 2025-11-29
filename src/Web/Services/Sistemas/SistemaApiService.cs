// =============================================================================
// RHSENSOERP WEB - SISTEMA API SERVICE
// =============================================================================
// Arquivo: src/Web/Services/Sistemas/SistemaApiService.cs
// Versão: 3.1 - Corrigido autenticação com GetTokenAsync
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sistemas;

/// <summary>
/// Serviço para comunicação com a API de Sistemas.
/// </summary>
public sealed class SistemaApiService : ISistemaApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SistemaApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string BaseEndpoint = "/api/identity/sistemas";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public SistemaApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<SistemaApiService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // =========================================================================
    // AUTENTICAÇÃO
    // =========================================================================

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // 1. Primeiro tenta via GetTokenAsync (onde StoreTokens salva)
            var token = await httpContext.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "access_token");

            // 2. Fallback: tenta via Claims
            if (string.IsNullOrEmpty(token))
            {
                token = httpContext.User.FindFirst("AccessToken")?.Value
                     ?? httpContext.User.FindFirst("access_token")?.Value;
            }

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("🔑 [SISTEMAS] Token JWT configurado para requisição");
            }
            else
            {
                _logger.LogWarning("⚠️ [SISTEMAS] Token JWT não encontrado para usuário autenticado");
            }
        }

        return client;
    }

    // =========================================================================
    // OPERAÇÕES CRUD
    // =========================================================================

    /// <inheritdoc/>
    public async Task<ApiResponse<PagedResult<SistemaDto>>> GetPagedAsync(
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

            _logger.LogDebug("[SISTEMAS] GET {Url}", url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializePagedResult(content);
            }

            _logger.LogWarning("[SISTEMAS] Erro ao buscar dados: {Status}", response.StatusCode);
            return CreateErrorResponse<PagedResult<SistemaDto>>("Erro ao buscar dados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao buscar dados paginados");
            return CreateErrorResponse<PagedResult<SistemaDto>>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<IEnumerable<SistemaDto>>> GetAllAsync()
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();
            var response = await client.GetAsync($"{BaseEndpoint}/all");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializeList(content);
            }

            return CreateErrorResponse<IEnumerable<SistemaDto>>("Erro ao buscar registros");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao buscar todos os registros");
            return CreateErrorResponse<IEnumerable<SistemaDto>>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<SistemaDto>> GetByIdAsync(string id)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();
            var url = $"{BaseEndpoint}/{Uri.EscapeDataString(id)}";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializeSingle(content);
            }

            return CreateErrorResponse<SistemaDto>("Registro não encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao buscar registro {Id}", id);
            return CreateErrorResponse<SistemaDto>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<SistemaDto>> CreateAsync(CreateSistemaDto dto)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var json = JsonSerializer.Serialize(dto, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(BaseEndpoint, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = TryDeserializeSingle(content);

                if (!result.Success || result.Data == null)
                {
                    return new ApiResponse<SistemaDto>
                    {
                        Success = true,
                        Data = new SistemaDto
                        {
                            CdSistema = dto.CdSistema,
                            DcSistema = dto.DcSistema,
                            Ativo = dto.Ativo
                        },
                        Error = new ApiError { Message = "Registro criado com sucesso" }
                    };
                }

                return result;
            }

            var errorMessage = TryExtractErrorMessage(content) ?? "Erro ao criar registro";
            return CreateErrorResponse<SistemaDto>(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao criar registro");
            return CreateErrorResponse<SistemaDto>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<SistemaDto>> UpdateAsync(string id, UpdateSistemaDto dto)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var json = JsonSerializer.Serialize(dto, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{BaseEndpoint}/{Uri.EscapeDataString(id)}";

            var response = await client.PutAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = TryDeserializeSingle(content);

                if (!result.Success || result.Data == null)
                {
                    return new ApiResponse<SistemaDto>
                    {
                        Success = true,
                        Data = new SistemaDto
                        {
                            CdSistema = id,  // Usa o ID da URL
                            DcSistema = dto.DcSistema,
                            Ativo = dto.Ativo
                        },
                        Error = new ApiError { Message = "Registro atualizado com sucesso" }
                    };
                }

                return result;
            }

            var errorMessage = TryExtractErrorMessage(content) ?? "Erro ao atualizar registro";
            return CreateErrorResponse<SistemaDto>(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao atualizar registro {Id}", id);
            return CreateErrorResponse<SistemaDto>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteAsync(string id)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var url = $"{BaseEndpoint}/{Uri.EscapeDataString(id)}";

            var response = await client.DeleteAsync(url);

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
            var errorMessage = TryExtractErrorMessage(content) ?? "Erro ao excluir registro";

            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = errorMessage }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao excluir registro {Id}", id);
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
        var result = await DeleteBatchAsync(ids);
        return new ApiResponse<bool>
        {
            Success = result.Success,
            Data = result.Success && (result.Data?.AllSucceeded ?? false),
            Error = result.Error
        };
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<string> codigos)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var codigosList = codigos.ToList();
            var json = JsonSerializer.Serialize(codigosList, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseEndpoint}/batch")
            {
                Content = httpContent
            };

            _logger.LogDebug("[SISTEMAS] DELETE {Url} | Payload: {Json}",
                $"{BaseEndpoint}/batch", json);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[SISTEMAS] DeleteBatch Response ({Status}): {Content}",
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializeBatchResult(content, codigosList.Count);
            }

            var errorMessage = TryExtractErrorMessage(content) ?? "Erro ao excluir registros";

            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = errorMessage }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao excluir múltiplos registros");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = ex.Message }
            };
        }
    }

    // =========================================================================
    // MÉTODOS AUXILIARES DE DESERIALIZAÇÃO
    // =========================================================================

    private ApiResponse<SistemaDto> TryDeserializeSingle(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Formato 1: Result<T> da API { isSuccess, value, error }
            if (root.TryGetProperty("isSuccess", out var isSuccessProp) ||
                root.TryGetProperty("IsSuccess", out isSuccessProp))
            {
                var isSuccess = isSuccessProp.GetBoolean();

                if (isSuccess)
                {
                    if (root.TryGetProperty("value", out var valueProp) ||
                        root.TryGetProperty("Value", out valueProp))
                    {
                        if (valueProp.ValueKind == JsonValueKind.Object)
                        {
                            var dto = JsonSerializer.Deserialize<SistemaDto>(
                                valueProp.GetRawText(), JsonOptions);

                            return new ApiResponse<SistemaDto>
                            {
                                Success = true,
                                Data = dto
                            };
                        }
                    }
                }
                else
                {
                    var errorMsg = TryExtractErrorFromElement(root);
                    return CreateErrorResponse<SistemaDto>(errorMsg ?? "Operação falhou");
                }
            }

            // Formato 2: Objeto direto (SistemaDto)
            if (root.TryGetProperty("cdSistema", out _) ||
                root.TryGetProperty("CdSistema", out _))
            {
                var dto = JsonSerializer.Deserialize<SistemaDto>(content, JsonOptions);
                return new ApiResponse<SistemaDto>
                {
                    Success = true,
                    Data = dto
                };
            }

            // Formato 3: Resposta encapsulada { success, data, message }
            if (root.TryGetProperty("success", out var successProp) ||
                root.TryGetProperty("Success", out successProp))
            {
                var success = successProp.GetBoolean();

                if (success && (root.TryGetProperty("data", out var dataProp) ||
                               root.TryGetProperty("Data", out dataProp)))
                {
                    if (dataProp.ValueKind == JsonValueKind.Object)
                    {
                        var dto = JsonSerializer.Deserialize<SistemaDto>(
                            dataProp.GetRawText(), JsonOptions);

                        return new ApiResponse<SistemaDto>
                        {
                            Success = true,
                            Data = dto
                        };
                    }
                }
            }

            return CreateErrorResponse<SistemaDto>("Formato de resposta não reconhecido");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao deserializar resposta");
            return CreateErrorResponse<SistemaDto>("Erro ao processar resposta da API");
        }
    }

    private ApiResponse<PagedResult<SistemaDto>> TryDeserializePagedResult(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("isSuccess", out var isSuccessProp) ||
                root.TryGetProperty("IsSuccess", out isSuccessProp))
            {
                if (isSuccessProp.GetBoolean())
                {
                    if (root.TryGetProperty("value", out var valueProp) ||
                        root.TryGetProperty("Value", out valueProp))
                    {
                        var pagedResult = JsonSerializer.Deserialize<PagedResult<SistemaDto>>(
                            valueProp.GetRawText(), JsonOptions);

                        return new ApiResponse<PagedResult<SistemaDto>>
                        {
                            Success = true,
                            Data = pagedResult
                        };
                    }
                }
            }

            if (root.TryGetProperty("items", out _) || root.TryGetProperty("Items", out _))
            {
                var pagedResult = JsonSerializer.Deserialize<PagedResult<SistemaDto>>(
                    content, JsonOptions);

                return new ApiResponse<PagedResult<SistemaDto>>
                {
                    Success = true,
                    Data = pagedResult
                };
            }

            return CreateErrorResponse<PagedResult<SistemaDto>>("Formato de resposta não reconhecido");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao deserializar resposta paginada");
            return CreateErrorResponse<PagedResult<SistemaDto>>("Erro ao processar resposta da API");
        }
    }

    private ApiResponse<IEnumerable<SistemaDto>> TryDeserializeList(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("isSuccess", out var isSuccessProp) ||
                root.TryGetProperty("IsSuccess", out isSuccessProp))
            {
                if (isSuccessProp.GetBoolean())
                {
                    if (root.TryGetProperty("value", out var valueProp) ||
                        root.TryGetProperty("Value", out valueProp))
                    {
                        var list = JsonSerializer.Deserialize<IEnumerable<SistemaDto>>(
                            valueProp.GetRawText(), JsonOptions);

                        return new ApiResponse<IEnumerable<SistemaDto>>
                        {
                            Success = true,
                            Data = list
                        };
                    }
                }
            }

            if (root.ValueKind == JsonValueKind.Array)
            {
                var list = JsonSerializer.Deserialize<IEnumerable<SistemaDto>>(
                    content, JsonOptions);

                return new ApiResponse<IEnumerable<SistemaDto>>
                {
                    Success = true,
                    Data = list
                };
            }

            return CreateErrorResponse<IEnumerable<SistemaDto>>("Formato de resposta não reconhecido");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao deserializar lista");
            return CreateErrorResponse<IEnumerable<SistemaDto>>("Erro ao processar resposta da API");
        }
    }

    private ApiResponse<BatchDeleteResultDto> TryDeserializeBatchResult(string content, int requestedCount)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Formato Result<BatchDeleteResult>: { isSuccess, value, error }
            if (root.TryGetProperty("isSuccess", out var isSuccessProp) ||
                root.TryGetProperty("IsSuccess", out isSuccessProp))
            {
                if (isSuccessProp.GetBoolean())
                {
                    if (root.TryGetProperty("value", out var valueProp) ||
                        root.TryGetProperty("Value", out valueProp))
                    {
                        var dto = JsonSerializer.Deserialize<BatchDeleteResultDto>(
                            valueProp.GetRawText(), JsonOptions);

                        var message = dto?.AllSucceeded == true
                            ? $"{dto.SuccessCount} registro(s) excluído(s) com sucesso."
                            : $"Excluídos: {dto?.SuccessCount}, Falhas: {dto?.FailureCount}";

                        return new ApiResponse<BatchDeleteResultDto>
                        {
                            Success = true,
                            Data = dto,
                            Error = new ApiError { Message = message }
                        };
                    }

                    // Se isSuccess mas não tem value, assume sucesso total
                    return new ApiResponse<BatchDeleteResultDto>
                    {
                        Success = true,
                        Data = new BatchDeleteResultDto
                        {
                            SuccessCount = requestedCount,
                            FailureCount = 0
                        },
                        Error = new ApiError { Message = $"{requestedCount} registro(s) excluído(s) com sucesso." }
                    };
                }
                else
                {
                    var errorMsg = TryExtractErrorFromElement(root);
                    return new ApiResponse<BatchDeleteResultDto>
                    {
                        Success = false,
                        Error = new ApiError { Message = errorMsg ?? "Erro na exclusão" }
                    };
                }
            }

            // Formato direto BatchDeleteResultDto
            if (root.TryGetProperty("successCount", out _) ||
                root.TryGetProperty("SuccessCount", out _))
            {
                var dto = JsonSerializer.Deserialize<BatchDeleteResultDto>(content, JsonOptions);
                return new ApiResponse<BatchDeleteResultDto>
                {
                    Success = true,
                    Data = dto,
                    Error = new ApiError { Message = $"{dto?.SuccessCount} registro(s) excluído(s)." }
                };
            }

            // Fallback: assume sucesso
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = true,
                Data = new BatchDeleteResultDto
                {
                    SuccessCount = requestedCount,
                    FailureCount = 0
                },
                Error = new ApiError { Message = $"{requestedCount} registro(s) excluído(s)." }
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[SISTEMAS] Erro ao deserializar resultado batch");
            return new ApiResponse<BatchDeleteResultDto>
            {
                Success = false,
                Error = new ApiError { Message = "Erro ao processar resposta da API" }
            };
        }
    }

    private string? TryExtractErrorMessage(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            return TryExtractErrorFromElement(doc.RootElement);
        }
        catch
        {
            return null;
        }
    }

    private string? TryExtractErrorFromElement(JsonElement element)
    {
        if (element.TryGetProperty("error", out var errorProp) ||
            element.TryGetProperty("Error", out errorProp))
        {
            if (errorProp.ValueKind == JsonValueKind.Object)
            {
                if (errorProp.TryGetProperty("description", out var descProp) ||
                    errorProp.TryGetProperty("Description", out descProp))
                {
                    return descProp.GetString();
                }

                if (errorProp.TryGetProperty("message", out var msgProp) ||
                    errorProp.TryGetProperty("Message", out msgProp))
                {
                    return msgProp.GetString();
                }
            }
            else if (errorProp.ValueKind == JsonValueKind.String)
            {
                return errorProp.GetString();
            }
        }

        if (element.TryGetProperty("message", out var directMsgProp) ||
            element.TryGetProperty("Message", out directMsgProp))
        {
            return directMsgProp.GetString();
        }

        return null;
    }

    private static ApiResponse<T> CreateErrorResponse<T>(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = new ApiError { Message = message }
        };
    }
}