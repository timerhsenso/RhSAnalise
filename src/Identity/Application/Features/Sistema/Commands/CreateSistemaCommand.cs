using AutoMapper;
using FluentValidation;
using MediatR;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Shared.Core.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Observação: removi o using RhSensoERP.Identity.Domain.Entities; aqui intencionalmente
// para forçar o uso de nome totalmente qualificado abaixo e evitar ambiguidade.
// Se a sua classe `Sistema` realmente estiver em `RhSensoERP.Identity.Domain.Entities`
// e não houver namespace chamado `Sistema`, você pode restaurar o using.

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

        // Uso do nome totalmente qualificado para evitar ambiguidade "Sistema (namespace vs tipo)".
        // Substitua o namespace abaixo pelo namespace real onde a classe Sistema está definida,
        // por exemplo: RhSensoERP.Identity.Domain.Models.Sistema ou RhSensoERP.Identity.Domain.Entities.Sistema
        var entity = _mapper.Map<RhSensoERP.Identity.Domain.Entities.Sistema>(request.Payload);

        await _repo.AddAsync(entity, ct);
        await _repo.UnitOfWork.SaveChangesAsync(ct);

        return Result<string>.Success(entity.CdSistema);
    }
}