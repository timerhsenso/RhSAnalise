using AutoMapper;
using MediatR;
using RhSensoERP.Identity.Application.DTOs.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Queries;

public sealed record GetSistemaByIdQuery(string CdSistema) : IRequest<Result<SistemaDto>>;

public sealed class GetSistemaByIdQueryHandler : IRequestHandler<GetSistemaByIdQuery, Result<SistemaDto>>
{
    private readonly ISistemaRepository _repo;
    private readonly IMapper _mapper;

    public GetSistemaByIdQueryHandler(ISistemaRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<SistemaDto>> Handle(GetSistemaByIdQuery request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.CdSistema, ct);
        if (entity is null) return Result<SistemaDto>.Failure("SISTEMA_NOT_FOUND", "Sistema não encontrado.");

        return Result<SistemaDto>.Success(_mapper.Map<SistemaDto>(entity));
    }
}
