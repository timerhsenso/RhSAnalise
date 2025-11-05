#nullable enable
using System;
namespace RhSensoERP.Shared.Application.Exceptions;
public class AppException : Exception
{
    public string? Code { get; }
    public AppException(string message, string? code = null) : base(message) => Code = code;
}
