#nullable enable
using System.Collections.Generic;
using RhSensoERP.Shared.Core.Common;
namespace RhSensoERP.Shared.Application.Exceptions;
public sealed class ValidationException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Ocorreram erros de validação.", ErrorCodes.Validation)
        => Errors = errors;
}
