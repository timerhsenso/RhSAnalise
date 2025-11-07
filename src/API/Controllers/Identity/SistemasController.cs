using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.DTOs.Sistema;
using RhSensoERP.Identity.Application.Features.Sistema.Commands;
using RhSensoERP.Identity.Application.Features.Sistema.Queries;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.API.Controllers.Identity;

[ApiController]
[Route("api/identity/sistemas")]
public sealed class SistemasController : ControllerBase
{
    private readonly IMediator _mediator;

    public SistemasController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{cdSistema}")]
    public async Task<ActionResult<Result<SistemaDto>>> GetById([FromRoute] string cdSistema, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSistemaByIdQuery(cdSistema), ct));

    [HttpGet]
    public async Task<ActionResult<Result<PagedResult<SistemaDto>>>> GetPaged([FromQuery] PagedRequest req, CancellationToken ct)
        => Ok(await _mediator.Send(new GetSistemasPagedQuery(req), ct));

    [HttpPost]
    public async Task<ActionResult<Result<string>>> Create([FromBody] CreateSistemaRequest body, CancellationToken ct)
        => Ok(await _mediator.Send(new CreateSistemaCommand(body), ct));

    [HttpPut("{cdSistema}")]
    public async Task<ActionResult<Result<bool>>> Update([FromRoute] string cdSistema, [FromBody] UpdateSistemaRequest body, CancellationToken ct)
        => Ok(await _mediator.Send(new UpdateSistemaCommand(cdSistema, body), ct));

    [HttpDelete("{cdSistema}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] string cdSistema, CancellationToken ct)
        => Ok(await _mediator.Send(new DeleteSistemaCommand(cdSistema), ct));
}
