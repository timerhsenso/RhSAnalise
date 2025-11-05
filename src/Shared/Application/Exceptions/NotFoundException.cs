#nullable enable
using RhSensoERP.Shared.Core.Common;
namespace RhSensoERP.Shared.Application.Exceptions;
public sealed class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, ErrorCodes.NotFound) { }
}
