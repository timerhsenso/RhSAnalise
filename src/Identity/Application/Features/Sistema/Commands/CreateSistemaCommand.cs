using AutoMapper;
using FluentValidation;
using MediatR;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Commands;

public sealed record CreateSistemaCommand(CreateSistemaRequest Payload) : IRequest<Result<string>>;

public sealed class CreateSistemaCommandHandler : IRequestHandler<CreateSistemaCommand, Result<string>>
{
    private readonly ISistemaRepository _repo;
    private readonly IValidator<CreateSistemaRequest> _validator;
    private readonly IMapper _mapper;

    public CreateSistemaCommandHandler(ISistemaRepository repo, IValidator<CreateSistemaRequest> validator, IMapper mapper)
    {
        _repo = repo;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<string>> Handle(CreateSistemaCommand request, CancellationToken ct)
    {
        var vr = await _validator.ValidateAsync(request.Payload, ct);
        if (!vr.IsValid) return Result<string>.Failure("VALIDATION_ERROR", string.Join("; ", vr.Errors.Select(e => e.ErrorMessage)));

        var exists = await _repo.ExistsAsync(request.Payload.CdSistema, ct);
        if (exists) return Result<string>.Failure("SISTEMA_ALREADY_EXISTS", "Já existe um sistema com esse código.");

        var entity = _mapper.Map<Sistema>(request.Payload);
        await _repo.AddAsync(entity, ct);
        await _repo.UnitOfWork.SaveChangesAsync(ct);

        return Result<string>.Success(entity.CdSistema);
    }
}
