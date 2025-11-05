namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Representa um erro no sistema.
/// </summary>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    /// Gets erro vazio (sem erro).
    /// </summary>
    public static Error None => new(string.Empty, string.Empty);

    /// <summary>
    /// Cria um erro de validação.
    /// </summary>
    public static Error Validation(string code, string message) => new(code, message);

    /// <summary>
    /// Cria um erro de não encontrado.
    /// </summary>
    public static Error NotFound(string code, string message) => new(code, message);

    /// <summary>
    /// Cria um erro de conflito.
    /// </summary>
    public static Error Conflict(string code, string message) => new(code, message);

    /// <summary>
    /// Cria um erro de falha.
    /// </summary>
    public static Error Failure(string code, string message) => new(code, message);
}
