namespace RhSensoERP.Shared.Contracts.Common;

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail<T>(string message)
        => new() { Success = false, Message = message };
}
