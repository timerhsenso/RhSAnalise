using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.Services;

namespace RhSensoERP.API.Controllers.Identity;

[ApiController]
[Route("api/identity/permissoes")]
public sealed class PermissoesController : ControllerBase
{
    private readonly IPermissaoService _service;
    public PermissoesController(IPermissaoService service) => _service = service;

    /// <summary>Permissões efetivas (funções + botões) do usuário, opcionalmente filtrando por sistema.</summary>
    [HttpGet("{cdUsuario}")]
    public async Task<IActionResult> GetPermissoes([FromRoute] string cdUsuario, [FromQuery] string? cdSistema, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario)) return BadRequest("cdUsuario obrigatório.");
        var result = await _service.GetPermissoesAsync(cdUsuario, cdSistema, ct);
        return Ok(result);
    }
}
