// =============================================================================
// RHSENSOERP CRUD TOOL - API CONTROLLER TEMPLATE
// =============================================================================
using RhSensoERP.CrudTool.Models;

namespace RhSensoERP.CrudTool.Templates;

public static class ApiControllerTemplate
{
    public static string Generate(EntityConfig entity)
    {
        var pkType = entity.PrimaryKey.Type;
        var pkName = entity.PrimaryKey.Property;
        var pkNameLower = char.ToLower(pkName[0]) + pkName.Substring(1);
        var moduleLower = entity.Module.ToLower();
        var pluralLower = entity.PluralName.ToLower();

        // IMPORTANTE: Namespaces usam PLURAL (entity.PluralName) - compatível com Source Generator
        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.{entity.Module}.Application.DTOs.Common;
using RhSensoERP.{entity.Module}.Application.DTOs.{entity.PluralName};
using RhSensoERP.{entity.Module}.Application.Features.{entity.PluralName}.Commands;
using RhSensoERP.{entity.Module}.Application.Features.{entity.PluralName}.Queries;
using RhSensoERP.Shared.Contracts.Common;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.API.Controllers.{entity.Module};

/// <summary>
/// Controller para gerenciamento de {entity.DisplayName}.
/// </summary>
[ApiController]
[Route(""api/{moduleLower}/{pluralLower}"")]
[ApiExplorerSettings(GroupName = ""{entity.Module}"")]
public sealed class {entity.PluralName}Controller : ControllerBase
{{
    private readonly IMediator _mediator;

    public {entity.PluralName}Controller(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Obtém {entity.DisplayName} pelo código.
    /// </summary>
    [HttpGet(""{{{pkNameLower}}}"")]
    public async Task<ActionResult<Result<{entity.Name}Dto>>> GetById(
        [FromRoute] {pkType} {pkNameLower},
        CancellationToken ct)
    {{
        var result = await _mediator.Send(new GetBy{entity.Name}IdQuery({pkNameLower}), ct);

        if (!result.IsSuccess)
        {{
            return NotFound(result);
        }}

        return Ok(result);
    }}

    /// <summary>
    /// Lista {entity.DisplayName} com paginação.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<PagedResult<{entity.Name}Dto>>>> GetPaged(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {{
        var result = await _mediator.Send(new Get{entity.PluralName}PagedQuery(req), ct);

        if (!result.IsSuccess)
        {{
            return BadRequest(result);
        }}

        return Ok(result);
    }}

    /// <summary>
    /// Cria novo {entity.DisplayName}.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<{pkType}>>> Create(
        [FromBody] Create{entity.Name}Request body,
        CancellationToken ct)
    {{
        var result = await _mediator.Send(new Create{entity.Name}Command(body), ct);

        if (!result.IsSuccess)
        {{
            return BadRequest(result);
        }}

        return Ok(result);
    }}

    /// <summary>
    /// Atualiza {entity.DisplayName} existente.
    /// </summary>
    [HttpPut(""{{{pkNameLower}}}"")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] {pkType} {pkNameLower},
        [FromBody] Update{entity.Name}Request body,
        CancellationToken ct)
    {{
        var result = await _mediator.Send(new Update{entity.Name}Command({pkNameLower}, body), ct);

        if (!result.IsSuccess)
        {{
            return BadRequest(result);
        }}

        return Ok(result);
    }}

    /// <summary>
    /// Remove {entity.DisplayName}.
    /// </summary>
    [HttpDelete(""{{{pkNameLower}}}"")]
    public async Task<ActionResult<Result<bool>>> Delete(
        [FromRoute] {pkType} {pkNameLower},
        CancellationToken ct)
    {{
        var result = await _mediator.Send(new Delete{entity.Name}Command({pkNameLower}), ct);

        if (!result.IsSuccess)
        {{
            return BadRequest(result);
        }}

        return Ok(result);
    }}

    /// <summary>
    /// Remove múltiplos {entity.DisplayName} em lote.
    /// </summary>
    [HttpDelete(""batch"")]
    public async Task<ActionResult<Result<BatchDeleteResult>>> DeleteMultiple(
        [FromBody] List<{pkType}> codigos,
        CancellationToken ct)
    {{
        var result = await _mediator.Send(new Delete{entity.PluralName}Command(codigos), ct);

        if (!result.IsSuccess)
        {{
            return BadRequest(result);
        }}

        return Ok(result);
    }}
}}
";
    }
}
