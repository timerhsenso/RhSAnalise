// =============================================================================
// RHSENSOERP CRUD TOOL - WEB SERVICES TEMPLATE
// =============================================================================
using RhSensoERP.CrudTool.Models;

namespace RhSensoERP.CrudTool.Templates;

public static class WebServicesTemplate
{
    /// <summary>
    /// Gera a interface do ApiService.
    /// </summary>
    public static string GenerateInterface(EntityConfig entity)
    {
        var pkType = entity.PrimaryKey.Type;

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using RhSensoERP.Web.Models.{entity.PluralName};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{entity.PluralName};

/// <summary>
/// Interface do serviço de API para {entity.DisplayName}.
/// </summary>
public interface I{entity.Name}ApiService 
    : IApiService<{entity.Name}Dto, Create{entity.Name}Dto, Update{entity.Name}Dto, {pkType}>,
      IBatchDeleteService<{pkType}>
{{
}}
";
    }

    /// <summary>
    /// Gera a implementação do ApiService.
    /// </summary>
    public static string GenerateImplementation(EntityConfig entity)
    {
        var pkType = entity.PrimaryKey.Type;
        var moduleLower = entity.Module.ToLower();
        var pluralLower = entity.PluralName.ToLower();
        var entityUpper = entity.Name.ToUpper();

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RhSensoERP.Web.Models.{entity.PluralName};
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.{entity.PluralName};

/// <summary>
/// Implementação do serviço de API para {entity.DisplayName}.
/// </summary>
public class {entity.Name}ApiService : I{entity.Name}ApiService
{{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<{entity.Name}ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private const string ApiRoute = ""api/{moduleLower}/{pluralLower}"";

    public {entity.Name}ApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<{entity.Name}ApiService> logger)
    {{
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {{
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }};
    }}

    #region Private Methods

    private void SetAuthHeader()
    {{
        var context = _httpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated == true)
        {{
            var token = context.User.FindFirst(""AccessToken"")?.Value;
            if (!string.IsNullOrEmpty(token))
            {{
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue(""Bearer"", token);
            }}
        }}
    }}

    private static ApiResponse<T> CreateErrorResponse<T>(string message)
    {{
        return new ApiResponse<T>
        {{
            Success = false,
            Error = new ApiError {{ Message = message }}
        }};
    }}

    private static ApiResponse<T> CreateSuccessResponse<T>(T? data)
    {{
        return new ApiResponse<T>
        {{
            Success = true,
            Data = data
        }};
    }}

    private async Task<ApiResponse<T>> DeserializeResponseAsync<T>(
        HttpResponseMessage response, 
        string operation)
    {{
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {{
            try
            {{
                // Tenta deserializar como ApiResponse<T> primeiro
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                if (apiResponse != null)
                    return apiResponse;

                // Se não for ApiResponse, tenta deserializar direto como T
                var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                return CreateSuccessResponse(data);
            }}
            catch (JsonException ex)
            {{
                _logger.LogWarning(ex, ""[{entityUpper}] Erro ao deserializar resposta de {{Operation}}"", operation);
                return CreateErrorResponse<T>(""Erro ao processar resposta do servidor"");
            }}
        }}

        // Tenta extrair mensagem de erro
        string? errorMessage = null;
        try
        {{
            var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(content, _jsonOptions);
            errorMessage = errorResponse?.Message;
        }}
        catch {{ }}

        _logger.LogWarning(""[{entityUpper}] Erro em {{Operation}}: {{Status}} - {{Content}}"", 
            operation, response.StatusCode, content);

        return CreateErrorResponse<T>(errorMessage ?? $""Erro: {{response.StatusCode}}"");
    }}

    #endregion

    #region IApiService Implementation

    /// <inheritdoc/>
    public async Task<ApiResponse<PagedResult<{entity.Name}Dto>>> GetPagedAsync(
        int page, int pageSize, string? search = null)
    {{
        try
        {{
            SetAuthHeader();

            // Monta query string
            var queryParams = $""?page={{page}}&pageSize={{pageSize}}"";
            if (!string.IsNullOrWhiteSpace(search))
            {{
                queryParams += $""&search={{Uri.EscapeDataString(search)}}"";
            }}

            var response = await _httpClient.GetAsync($""{{ApiRoute}}{{queryParams}}"");
            return await DeserializeResponseAsync<PagedResult<{entity.Name}Dto>>(response, ""GetPaged"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetPagedAsync"");
            return CreateErrorResponse<PagedResult<{entity.Name}Dto>>(""Erro de conexão com o servidor"");
        }}
    }}

    /// <inheritdoc/>
    public async Task<ApiResponse<IEnumerable<{entity.Name}Dto>>> GetAllAsync()
    {{
        try
        {{
            SetAuthHeader();

            // Busca todos usando paginação grande
            var result = await GetPagedAsync(1, 10000, null);
            
            if (!result.Success || result.Data == null)
            {{
                return CreateErrorResponse<IEnumerable<{entity.Name}Dto>>(result.Message ?? ""Erro ao buscar dados"");
            }}

            return CreateSuccessResponse<IEnumerable<{entity.Name}Dto>>(result.Data.Items);
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetAllAsync"");
            return CreateErrorResponse<IEnumerable<{entity.Name}Dto>>(""Erro de conexão com o servidor"");
        }}
    }}

    /// <inheritdoc/>
    public async Task<ApiResponse<{entity.Name}Dto>> GetByIdAsync({pkType} id)
    {{
        try
        {{
            SetAuthHeader();

            var response = await _httpClient.GetAsync($""{{ApiRoute}}/{{id}}"");
            return await DeserializeResponseAsync<{entity.Name}Dto>(response, ""GetById"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em GetByIdAsync"");
            return CreateErrorResponse<{entity.Name}Dto>(""Erro de conexão com o servidor"");
        }}
    }}

    /// <inheritdoc/>
    public async Task<ApiResponse<{entity.Name}Dto>> CreateAsync(Create{entity.Name}Dto dto)
    {{
        try
        {{
            SetAuthHeader();

            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");

            var response = await _httpClient.PostAsync(ApiRoute, content);
            return await DeserializeResponseAsync<{entity.Name}Dto>(response, ""Create"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em CreateAsync"");
            return CreateErrorResponse<{entity.Name}Dto>(""Erro de conexão com o servidor"");
        }}
    }}

    /// <inheritdoc/>
    public async Task<ApiResponse<{entity.Name}Dto>> UpdateAsync({pkType} id, Update{entity.Name}Dto dto)
    {{
        try
        {{
            SetAuthHeader();

            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, ""application/json"");

            var response = await _httpClient.PutAsync($""{{ApiRoute}}/{{id}}"", content);
            return await DeserializeResponseAsync<{entity.Name}Dto>(response, ""Update"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em UpdateAsync"");
            return CreateErrorResponse<{entity.Name}Dto>(""Erro de conexão com o servidor"");
        }}
    }}

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteAsync({pkType} id)
    {{
        try
        {{
            SetAuthHeader();

            var response = await _httpClient.DeleteAsync($""{{ApiRoute}}/{{id}}"");
            
            if (response.IsSuccessStatusCode)
            {{
                return CreateSuccessResponse(true);
            }}

            return await DeserializeResponseAsync<bool>(response, ""Delete"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteAsync"");
            return CreateErrorResponse<bool>(""Erro de conexão com o servidor"");
        }}
    }}

    /// <inheritdoc/>
    public async Task<ApiResponse<bool>> DeleteMultipleAsync(IEnumerable<{pkType}> ids)
    {{
        try
        {{
            var result = await DeleteBatchAsync(ids);
            
            return new ApiResponse<bool>
            {{
                Success = result.Success,
                Data = result.Success && (result.Data?.AllSucceeded ?? false),
                Error = result.Error
            }};
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteMultipleAsync"");
            return CreateErrorResponse<bool>(""Erro de conexão com o servidor"");
        }}
    }}

    #endregion

    #region IBatchDeleteService Implementation

    /// <inheritdoc/>
    public async Task<ApiResponse<BatchDeleteResultDto>> DeleteBatchAsync(IEnumerable<{pkType}> ids)
    {{
        try
        {{
            SetAuthHeader();

            var idsList = ids.ToList();
            var json = JsonSerializer.Serialize(idsList, _jsonOptions);
            
            // HTTP DELETE com body
            var request = new HttpRequestMessage(HttpMethod.Delete, $""{{ApiRoute}}/batch"")
            {{
                Content = new StringContent(json, Encoding.UTF8, ""application/json"")
            }};

            var response = await _httpClient.SendAsync(request);
            return await DeserializeResponseAsync<BatchDeleteResultDto>(response, ""DeleteBatch"");
        }}
        catch (Exception ex)
        {{
            _logger.LogError(ex, ""[{entityUpper}] Exceção em DeleteBatchAsync"");
            return CreateErrorResponse<BatchDeleteResultDto>(""Erro de conexão com o servidor"");
        }}
    }}

    #endregion
}}
";
    }
}
