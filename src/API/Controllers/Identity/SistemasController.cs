// src/API/Controllers/Identity/SistemasController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.DTOs.Common;
using RhSensoERP.Identity.Application.DTOs.Sistema;
using RhSensoERP.Identity.Application.Features.Sistema.Commands;
using RhSensoERP.Identity.Application.Features.Sistema.Queries;
using RhSensoERP.Identity.Application.Requests.Sistema;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para gerenciamento de sistemas do ERP.
/// </summary>
[ApiController]
[Route("api/identity/sistemas")]
[ApiExplorerSettings(GroupName = "Identity")]
public sealed class SistemasController : ControllerBase
{
    private readonly IMediator _mediator;

    public SistemasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Obtem um sistema pelo codigo.
    /// </summary>
    [HttpGet("{cdSistema}")]
    public async Task<ActionResult<Result<SistemaDto>>> GetById(
        [FromRoute] string cdSistema,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSistemaByIdQuery(cdSistema), ct);

        // ✅ Verifica se houve erro
        if (!result.IsSuccess)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista sistemas com paginacao.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<PagedResult<SistemaDto>>>> GetPaged(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSistemasPagedQuery(req), ct);

        // ✅ Verifica se houve erro
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cria novo sistema.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<string>>> Create(
        [FromBody] CreateSistemaRequest body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateSistemaCommand(body), ct);

        // ✅ CORREÇÃO PRINCIPAL: Verifica se houve erro de validação (ex: código duplicado)
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Atualiza sistema existente.
    /// </summary>
    [HttpPut("{cdSistema}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] string cdSistema,
        [FromBody] UpdateSistemaRequest body,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateSistemaCommand(cdSistema, body), ct);

        // ✅ Verifica se houve erro
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove sistema.
    /// </summary>
    [HttpDelete("{cdSistema}")]
    public async Task<ActionResult<Result<bool>>> Delete(
        [FromRoute] string cdSistema,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSistemaCommand(cdSistema), ct);

        // ✅ Verifica se houve erro
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Remove multiplos sistemas em lote.
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<Result<BatchDeleteResult>>> DeleteMultiple(
        [FromBody] List<string> codigos,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteSistemasCommand(codigos), ct);

        // ✅ Verifica se houve erro
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
