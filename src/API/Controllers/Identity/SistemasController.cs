// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using MediatR;
using Microsoft.AspNetCore.Mvc;
//using RhSensoERP.Identity.Application.DTOs.Common;
using RhSensoERP.Shared.Application.DTOs.Common;

using RhSensoERP.Identity.Application.DTOs.Sistemas;
using RhSensoERP.Identity.Application.Features.Sistemas.Commands;
using RhSensoERP.Identity.Application.Features.Sistemas.Queries;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para gerenciamento de Sistema.
/// </summary>
[ApiController]
[Route("api/identity/sistemas")]
[ApiExplorerSettings(GroupName = "Identity")]
public sealed class SistemasController : ControllerBase
{
    private readonly IMediator _mediator;

    public SistemasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Obtém Sistema pelo código.
    /// </summary>
    [HttpGet("{cdSistema}")]
    public async Task<ActionResult<Result<SistemaDto>>> GetById(
        [FromRoute] string cdSistema,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBySistemaIdQuery(cdSistema), ct);

        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista Sistema com paginação.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<PagedResult<SistemaDto>>>> GetPaged(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSistemasPagedQuery(req), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria novo Sistema.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Create(
        [FromBody] CreateSistemaRequest body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSistemaCommand(body), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Atualiza Sistema existente.
    /// </summary>
    [HttpPut("{cdSistema}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] string cdSistema,
        [FromBody] UpdateSistemaRequest body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateSistemaCommand(cdSistema, body), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove Sistema.
    /// </summary>
    [HttpDelete("{cdSistema}")]
    public async Task<ActionResult<Result<bool>>> Delete(
        [FromRoute] string cdSistema,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSistemaCommand(cdSistema), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove múltiplos Sistema em lote.
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<Result<BatchDeleteResult>>> DeleteMultiple(
        [FromBody] List<string> codigos,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSistemasCommand(codigos), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
