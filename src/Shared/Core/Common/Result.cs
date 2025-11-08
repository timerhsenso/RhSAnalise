namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Resultado padrão (Success/Failure) com payload opcional.
/// </summary>
public sealed class Result<T>
{
    public bool Succeeded { get; init; }
    public string? Code { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static Result<T> Success(T data) =>
        new() { Succeeded = true, Data = data };

    public static Result<T> Failure(string message) =>
        new() { Succeeded = false, Message = message };

    /// <summary>
    /// Falha com código e mensagem (usado pelos seus handlers).
    /// </summary>
    public static Result<T> Failure(string code, string message) =>
        new() { Succeeded = false, Code = code, Message = message };
}
