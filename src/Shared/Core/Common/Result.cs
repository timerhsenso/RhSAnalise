// Copyright (c) RhSenso. Todos os direitos reservados.

namespace RhSensoERP.Shared.Core.Common;

/// <summary>Resultado de operação sem payload.</summary>
public sealed class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Ok() => new(true, null);

    public static Result Failure(string error) => new(false, error);
}

/// <summary>Resultado de operação com payload.</summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(true, value, null);

    public static Result<T> Failure(string error) => new(false, default, error);
}
