// src/Modules/Identity/RhSensoERP.Identity.Application/DTOs/Common/BatchDeleteResult.cs
namespace RhSensoERP.Identity.Application.DTOs.Common;

/// <summary>
/// Resultado de operacao de exclusao em lote.
/// </summary>
public sealed record BatchDeleteResult
{
    /// <summary>
    /// Quantidade de registros excluidos com sucesso.
    /// </summary>
    public int SuccessCount { get; init; }

    /// <summary>
    /// Quantidade de registros que falharam.
    /// </summary>
    public int FailureCount { get; init; }

    /// <summary>
    /// Total de registros processados.
    /// </summary>
    public int TotalCount => SuccessCount + FailureCount;

    /// <summary>
    /// Lista de erros por codigo.
    /// </summary>
    public List<BatchDeleteError> Errors { get; init; } = [];

    /// <summary>
    /// Indica se todas as exclusoes foram bem-sucedidas.
    /// </summary>
    public bool AllSucceeded => FailureCount == 0;
}

/// <summary>
/// Erro de exclusao individual.
/// </summary>
public sealed record BatchDeleteError(string Code, string Message);