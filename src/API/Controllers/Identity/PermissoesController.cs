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

    public PermissoesController(IPermissaoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna todas as permissões (funções e botões) de um usuário,
    /// opcionalmente filtrando por sistema.
    /// </summary>
    /// <param name="cdUsuario">Código do usuário legado (tuse1.cdusuario)</param>
    /// <param name="cdSistema">Código do sistema (opcional)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>DTO com permissões do usuário</returns>
    [HttpGet("{cdUsuario}")]
    public async Task<IActionResult> GetPermissoes(
        [FromRoute] string cdUsuario,
        [FromQuery] string? cdSistema,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            return BadRequest(new { error = "cdUsuario obrigatório." });

        var result = await _service.CarregarPermissoesAsync(cdUsuario, cdSistema, ct);
        return Ok(result);
    }
}
