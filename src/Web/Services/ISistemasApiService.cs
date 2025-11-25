// src/Web/Services/ISistemasApiService.cs

using RhSensoERP.Web.Models.Identity;

namespace RhSensoERP.Web.Services;

/// <summary>
/// Interface para comunicação com a API de Sistemas.
/// </summary>
public interface ISistemasApiService
{
    /// <summary>
    /// Obtém lista paginada de sistemas.
    /// </summary>
    Task<PagedResultViewModel<SistemaViewModel>?> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        bool? ativo = null,
        CancellationToken ct = default);

    /// <summary>
    /// Obtém um sistema pelo código.
    /// </summary>
    Task<SistemaViewModel?> GetByIdAsync(string cdSistema, CancellationToken ct = default);

    /// <summary>
    /// Cria um novo sistema.
    /// </summary>
    Task<ApiResultViewModel<string>?> CreateAsync(CreateSistemaViewModel model, CancellationToken ct = default);

    /// <summary>
    /// Atualiza um sistema existente.
    /// </summary>
    Task<ApiResultViewModel<bool>?> UpdateAsync(string cdSistema, UpdateSistemaViewModel model, CancellationToken ct = default);

    /// <summary>
    /// Exclui um sistema.
    /// </summary>
    Task<ApiResultViewModel<bool>?> DeleteAsync(string cdSistema, CancellationToken ct = default);
}

/// <summary>
/// ViewModel genérico para resultado paginado.
/// </summary>
public sealed class PagedResultViewModel<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

/// <summary>
/// ViewModel genérico para resultado de API.
/// </summary>
public sealed class ApiResultViewModel<T>
{
    public bool IsSuccess { get; set; }
    public T? Value { get; set; }
    public ApiErrorViewModel? Error { get; set; }
}

/// <summary>
/// ViewModel para erro de API.
/// </summary>
public sealed class ApiErrorViewModel
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
