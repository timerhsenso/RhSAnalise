// src/Web/Models/Common/ApiResponse.cs

namespace RhSensoERP.Web.Models.Common;

/// <summary>
/// Resposta padronizada da API.
/// </summary>
/// <typeparam name="T">Tipo do dado retornado</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Dados retornados.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Erros de validação (se houver).
    /// </summary>
    public Dictionary<string, List<string>>? Errors { get; set; }
}

/// <summary>
/// Resposta paginada da API.
/// </summary>
/// <typeparam name="T">Tipo do item da lista</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Lista de itens da página atual.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Número da página atual.
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indica se há página anterior.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indica se há próxima página.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}
