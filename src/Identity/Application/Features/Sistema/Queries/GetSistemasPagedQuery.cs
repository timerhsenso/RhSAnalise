using AutoMapper;
using MediatR;
using RhSensoERP.Identity.Application.DTOs.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Identity.Application.Features.Sistema.Queries;

/// <summary>Query paginada de Sistemas (com busca).</summary>
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
        var result = await _repo.ListPagedAsync(
            request.Request.Page, request.Request.PageSize, request.Request.Search, ct);

        var items = result.Items;
        var total = result.TotalCount;

        var data = items.Select(_mapper.Map<SistemaDto>).ToList();
        var paged = new PagedResult<SistemaDto>(data, total, request.Request.Page, request.Request.PageSize);
        return Result<PagedResult<SistemaDto>>.Success(paged);
    }
}
