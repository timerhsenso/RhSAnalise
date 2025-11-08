using AutoMapper;
using FluentValidation;
using MediatR;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Commands;

/// <summary>Command para atualizar um Sistema.</summary>
public sealed record UpdateSistemaCommand(string CdSistema, UpdateSistemaRequest Payload) : IRequest<Result<bool>>;

public sealed class UpdateSistemaCommandHandler : IRequestHandler<UpdateSistemaCommand, Result<bool>>
{
    private readonly ISistemaRepository _repo;
    private readonly IValidator<UpdateSistemaRequest> _validator;
    private readonly IMapper _mapper;

    public UpdateSistemaCommandHandler(ISistemaRepository repo, IValidator<UpdateSistemaRequest> validator, IMapper mapper)
    {
        _repo = repo;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<bool>> Handle(UpdateSistemaCommand request, CancellationToken ct)
    {
        var vr = await _validator.ValidateAsync(request.Payload, ct);
        if (!vr.IsValid)
            return Result<bool>.Failure("VALIDATION_ERROR", string.Join("; ", vr.Errors.Select(e => e.ErrorMessage)));

        var entity = await _repo.GetByIdAsync(request.CdSistema, ct);
        if (entity is null)
            return Result<bool>.Failure("SISTEMA_NOT_FOUND", "Sistema não encontrado.");

        _mapper.Map(request.Payload, entity);
        _repo.Update(entity);
        await _repo.UnitOfWork.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}
