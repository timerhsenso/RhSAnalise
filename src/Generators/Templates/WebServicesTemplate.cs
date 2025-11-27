// =============================================================================
// RHSENSOERP GENERATOR v3.0 - WEB SERVICES TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Web Services (Interface + Implementação).
/// NOVO no v3.0!
/// </summary>
public static class WebServicesTemplate
{
    /// <summary>
    /// Gera a Interface do Service.
    /// </summary>
    public static string GenerateInterface(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using RhSensoERP.Web.Models.Common;
using {{info.WebModelsNamespace}};
using RhSensoERP.Web.Services.Base;

namespace {{info.WebServicesNamespace}};

/// <summary>
/// Interface do serviço para comunicação com a API de {{info.DisplayName}}.
/// Implementa IBatchDeleteService para suportar exclusão em lote com resultado detalhado.
/// </summary>
public interface I{{info.EntityName}}ApiService :
    IApiService<{{info.EntityName}}Dto, Create{{info.EntityName}}Dto, Update{{info.EntityName}}Dto, {{pkType}}>,
    IBatchDeleteService<{{pkType}}>
{
    // A interface IBatchDeleteService já define o método DeleteBatchAsync
    // Não é necessário redeclarar aqui, pois a herança já o inclui
}
""";
    }

    /// <summary>
    /// Gera a Implementação do Service.
    /// </summary>
    public static string GenerateImplementation(EntityInfo info)
    {
        var pkType = info.PrimaryKeyType;
        var idEscape = pkType == "string" ? "Uri.EscapeDataString(id)" : "id";
        var entityUpper = info.EntityName.ToUpperInvariant();

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RhSensoERP.Web.Models.Common;
using {{info.WebModelsNamespace}};
using RhSensoERP.Web.Services.Base;

namespace {{info.WebServicesNamespace}};

/// <summary>
/// Serviço para comunicação com a API de {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}ApiService : I{{info.EntityName}}ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<{{info.EntityName}}ApiService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string BaseEndpoint = "{{info.ApiFullRoute}}";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public {{info.EntityName}}ApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<{{info.EntityName}}ApiService> logger,
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

    // =========================================================================
    // OPERAÇÕES CRUD
    // =========================================================================

    /// <inheritdoc/>
    public async Task<ApiResponse<PagedResult<{{info.EntityName}}Dto>>> GetPagedAsync(
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

            _logger.LogDebug("[{{entityUpper}}] GET {Url}", url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializePagedResult(content);
            }

            _logger.LogWarning("[{{entityUpper}}] Erro ao buscar dados: {Status}", response.StatusCode);
            return CreateErrorResponse<PagedResult<{{info.EntityName}}Dto>>("Erro ao buscar dados");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao buscar dados paginados");
            return CreateErrorResponse<PagedResult<{{info.EntityName}}Dto>>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<IEnumerable<{{info.EntityName}}Dto>>> GetAllAsync()
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

            return CreateErrorResponse<IEnumerable<{{info.EntityName}}Dto>>("Erro ao buscar registros");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao buscar todos os registros");
            return CreateErrorResponse<IEnumerable<{{info.EntityName}}Dto>>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<{{info.EntityName}}Dto>> GetByIdAsync({{pkType}} id)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();
            var url = $"{BaseEndpoint}/{{{idEscape}}}";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializeSingle(content);
            }

            return CreateErrorResponse<{{info.EntityName}}Dto>("Registro nao encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao buscar registro {Id}", id);
            return CreateErrorResponse<{{info.EntityName}}Dto>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<{{info.EntityName}}Dto>> CreateAsync(Create{{info.EntityName}}Dto dto)
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
                    return new ApiResponse<{{info.EntityName}}Dto>
                    {
                        Success = true,
                        Error = new ApiError { Message = "Registro criado com sucesso" }
                    };
                }

                return result;
            }

            var errorMessage = TryExtractErrorMessage(content) ?? "Erro ao criar registro";
            return CreateErrorResponse<{{info.EntityName}}Dto>(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao criar registro");
            return CreateErrorResponse<{{info.EntityName}}Dto>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<{{info.EntityName}}Dto>> UpdateAsync({{pkType}} id, Update{{info.EntityName}}Dto dto)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var json = JsonSerializer.Serialize(dto, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{BaseEndpoint}/{{{idEscape}}}";

            var response = await client.PutAsync(url, httpContent);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = TryDeserializeSingle(content);

                if (!result.Success || result.Data == null)
                {
                    return new ApiResponse<{{info.EntityName}}Dto>
                    {
                        Success = true,
                        Error = new ApiError { Message = "Registro atualizado com sucesso" }
                    };
                }

                return result;
            }

            var errorMessage = TryExtractErrorMessage(content) ?? "Erro ao atualizar registro";
            return CreateErrorResponse<{{info.EntityName}}Dto>(errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao atualizar registro {Id}", id);
            return CreateErrorResponse<{{info.EntityName}}Dto>(ex.Message);
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteAsync({{pkType}} id)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var url = $"{BaseEndpoint}/{{{idEscape}}}";

            var response = await client.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Error = new ApiError { Message = "Registro excluido com sucesso" }
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
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao excluir registro {Id}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Error = new ApiError { Message = ex.Message }
            };
        }
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<{{pkType}}> ids)
    {
        // Implementacao simples que chama DeleteBatchAsync
        var result = await DeleteBatchAsync(ids);
        return new ApiResponse<bool>
        {
            Success = result.Success,
            Data = result.Success && (result.Data?.AllSucceeded ?? false),
            Error = result.Error
        };
    }

    /// <inheritdoc/>
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<{{pkType}}> ids)
    {
        try
        {
            var client = await GetAuthenticatedClientAsync();

            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, JsonOptions);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Endpoint correto: /batch
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseEndpoint}/batch")
            {
                Content = httpContent
            };

            _logger.LogDebug("[{{entityUpper}}] DELETE {Url} | Payload: {Json}",
                $"{BaseEndpoint}/batch", json);

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("[{{entityUpper}}] DeleteBatch Response ({Status}): {Content}",
                response.StatusCode, content);

            if (response.IsSuccessStatusCode)
            {
                return TryDeserializeBatchResult(content, idsList.Count);
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
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao excluir multiplos registros");
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

    private ApiResponse<{{info.EntityName}}Dto> TryDeserializeSingle(string content)
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
                            var dto = JsonSerializer.Deserialize<{{info.EntityName}}Dto>(
                                valueProp.GetRawText(), JsonOptions);

                            return new ApiResponse<{{info.EntityName}}Dto>
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
                    return CreateErrorResponse<{{info.EntityName}}Dto>(errorMsg ?? "Erro desconhecido");
                }
            }

            // Formato 2: DTO direto
            var directDto = JsonSerializer.Deserialize<{{info.EntityName}}Dto>(content, JsonOptions);
            return new ApiResponse<{{info.EntityName}}Dto>
            {
                Success = true,
                Data = directDto
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao deserializar resposta");
            return CreateErrorResponse<{{info.EntityName}}Dto>("Erro ao processar resposta da API");
        }
    }

    private ApiResponse<PagedResult<{{info.EntityName}}Dto>> TryDeserializePagedResult(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Formato Result<PagedResult<T>>: { isSuccess, value, error }
            if (root.TryGetProperty("isSuccess", out var isSuccessProp) ||
                root.TryGetProperty("IsSuccess", out isSuccessProp))
            {
                if (isSuccessProp.GetBoolean())
                {
                    if (root.TryGetProperty("value", out var valueProp) ||
                        root.TryGetProperty("Value", out valueProp))
                    {
                        var pagedResult = JsonSerializer.Deserialize<PagedResult<{{info.EntityName}}Dto>>(
                            valueProp.GetRawText(), JsonOptions);

                        return new ApiResponse<PagedResult<{{info.EntityName}}Dto>>
                        {
                            Success = true,
                            Data = pagedResult
                        };
                    }
                }
            }

            // Formato direto PagedResult
            if (root.TryGetProperty("items", out _) || root.TryGetProperty("Items", out _))
            {
                var pagedResult = JsonSerializer.Deserialize<PagedResult<{{info.EntityName}}Dto>>(
                    content, JsonOptions);

                return new ApiResponse<PagedResult<{{info.EntityName}}Dto>>
                {
                    Success = true,
                    Data = pagedResult
                };
            }

            return CreateErrorResponse<PagedResult<{{info.EntityName}}Dto>>("Formato de resposta nao reconhecido");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao deserializar resposta paginada");
            return CreateErrorResponse<PagedResult<{{info.EntityName}}Dto>>("Erro ao processar resposta da API");
        }
    }

    private ApiResponse<IEnumerable<{{info.EntityName}}Dto>> TryDeserializeList(string content)
    {
        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Formato Result<IEnumerable<T>>
            if (root.TryGetProperty("isSuccess", out var isSuccessProp) ||
                root.TryGetProperty("IsSuccess", out isSuccessProp))
            {
                if (isSuccessProp.GetBoolean())
                {
                    if (root.TryGetProperty("value", out var valueProp) ||
                        root.TryGetProperty("Value", out valueProp))
                    {
                        var list = JsonSerializer.Deserialize<IEnumerable<{{info.EntityName}}Dto>>(
                            valueProp.GetRawText(), JsonOptions);

                        return new ApiResponse<IEnumerable<{{info.EntityName}}Dto>>
                        {
                            Success = true,
                            Data = list
                        };
                    }
                }
            }

            // Formato direto array
            if (root.ValueKind == JsonValueKind.Array)
            {
                var list = JsonSerializer.Deserialize<IEnumerable<{{info.EntityName}}Dto>>(
                    content, JsonOptions);

                return new ApiResponse<IEnumerable<{{info.EntityName}}Dto>>
                {
                    Success = true,
                    Data = list
                };
            }

            return CreateErrorResponse<IEnumerable<{{info.EntityName}}Dto>>("Formato de resposta nao reconhecido");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao deserializar lista");
            return CreateErrorResponse<IEnumerable<{{info.EntityName}}Dto>>("Erro ao processar resposta da API");
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
                            ? $"{dto.SuccessCount} registro(s) excluido(s) com sucesso."
                            : $"Excluidos: {dto?.SuccessCount}, Falhas: {dto?.FailureCount}";

                        return new ApiResponse<BatchDeleteResultDto>
                        {
                            Success = true,
                            Data = dto,
                            Error = new ApiError { Message = message }
                        };
                    }

                    // Se isSuccess mas nao tem value, assume sucesso total
                    return new ApiResponse<BatchDeleteResultDto>
                    {
                        Success = true,
                        Data = new BatchDeleteResultDto
                        {
                            SuccessCount = requestedCount,
                            FailureCount = 0
                        },
                        Error = new ApiError { Message = $"{requestedCount} registro(s) excluido(s) com sucesso." }
                    };
                }
                else
                {
                    var errorMsg = TryExtractErrorFromElement(root);
                    return new ApiResponse<BatchDeleteResultDto>
                    {
                        Success = false,
                        Error = new ApiError { Message = errorMsg ?? "Erro na exclusao" }
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
                    Error = new ApiError { Message = $"{dto?.SuccessCount} registro(s) excluido(s)." }
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
                Error = new ApiError { Message = $"{requestedCount} registro(s) excluido(s)." }
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[{{entityUpper}}] Erro ao deserializar resultado batch");
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
""";
    }
}
