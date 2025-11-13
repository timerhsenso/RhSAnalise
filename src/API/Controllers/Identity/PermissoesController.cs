// src/API/Controllers/Identity/PermissoesController.cs

using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.Services;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para gerenciamento de permissões de usuários.
/// </summary>
[ApiController]
[Route("api/identity/permissoes")]
[ApiExplorerSettings(GroupName = "Identity")]
public sealed class PermissoesController : ControllerBase
{
    private readonly IPermissaoService _service;

    public PermissoesController(IPermissaoService service) => _service = service;

    /// <summary>
    /// Obtém permissões efetivas (funções + botões) do usuário.
    /// Permite filtrar opcionalmente por sistema específico.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista de permissões do usuário</returns>
    [HttpGet("{cdUsuario}")]
    public async Task<IActionResult> GetPermissoes(
        [FromRoute] string cdUsuario,
        [FromQuery] string? cdSistema,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            return BadRequest(new { error = "cdUsuario obrigatório." });

        var result = await _service.GetPermissoesAsync(cdUsuario, cdSistema, ct);
        return Ok(result);
    }
}