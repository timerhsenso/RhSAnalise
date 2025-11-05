namespace RhSensoERP.Shared.Core.Common;

/// <summary>
/// Representa o resultado de uma operação.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether a operação foi bem-sucedida.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets o erro da operação, se houver.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Cria um resultado de sucesso.
    /// </summary>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Cria um resultado de falha.
    /// </summary>
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// Representa o resultado de uma operação com valor de retorno.
/// </summary>
/// <typeparam name="T">Tipo do valor retornado.</typeparam>
public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Gets o valor retornado pela operação.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Cria um resultado de sucesso com valor.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, Error.None);

    /// <summary>
    /// Cria um resultado de falha.
    /// </summary>
    public static new Result<T> Failure(Error error) => new(false, default, error);
}
