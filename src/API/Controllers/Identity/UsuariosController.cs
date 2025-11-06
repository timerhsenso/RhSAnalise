using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Identity.Application.Services;

namespace RhSensoERP.API.Controllers.Identity;

[ApiController]
[Route("api/identity/usuarios")]
public sealed class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioService service) => _service = service;

    /// <summary>Obtém um usuário pelo código (cdusuario).</summary>
    [HttpGet("{cdUsuario}")]
    public async Task<IActionResult> GetByCodigo([FromRoute] string cdUsuario, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cdUsuario))
            return BadRequest("cdUsuario obrigatório.");

        var result = await _service.GetAsync(cdUsuario, ct);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Pesquisa usuários por termo (cdusuario, nome ou e-mail).</summary>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? term, [FromQuery] int take = 20, CancellationToken ct = default)
    {
        var result = await _service.SearchAsync(term, take, ct);
        return Ok(result);
    }
}
