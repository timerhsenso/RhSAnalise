using MediatR;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Commands;

public sealed record DeleteSistemaCommand(string CdSistema) : IRequest<Result<bool>>;

public sealed class DeleteSistemaCommandHandler : IRequestHandler<DeleteSistemaCommand, Result<bool>>
{
    private readonly ISistemaRepository _repo;

    public DeleteSistemaCommandHandler(ISistemaRepository repo) => _repo = repo;

    public async Task<Result<bool>> Handle(DeleteSistemaCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.CdSistema, ct);
        if (entity is null) return Result<bool>.Failure("SISTEMA_NOT_FOUND", "Sistema não encontrado.");

        _repo.Delete(entity);
        await _repo.UnitOfWork.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
