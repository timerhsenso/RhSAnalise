using AutoMapper;
using MediatR;
using RhSensoERP.Identity.Application.DTOs.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Queries;

public sealed record GetSistemasPagedQuery(PagedRequest Request) : IRequest<Result<PagedResult<SistemaDto>>>;

public sealed class GetSistemasPagedQueryHandler
    : IRequestHandler<GetSistemasPagedQuery, Result<PagedResult<SistemaDto>>>
{
    private readonly ISistemaRepository _repo;
    private readonly IMapper _mapper;

    public GetSistemasPagedQueryHandler(ISistemaRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<SistemaDto>>> Handle(GetSistemasPagedQuery request, CancellationToken ct)
    {
        var (items, total) = await _repo.ListPagedAsync(
            request.Request.Page, request.Request.PageSize, request.Request.Search, ct);

        var data = items.Select(_mapper.Map<SistemaDto>).ToList();
        return Result<PagedResult<SistemaDto>>.Success(new PagedResult<SistemaDto>(data, total, request.Request.Page, request.Request.PageSize));
    }
}
