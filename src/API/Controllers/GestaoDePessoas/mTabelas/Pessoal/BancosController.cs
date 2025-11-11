using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Application.Services;
using RhSensoERP.Shared.Contracts.Common;

namespace RhSensoERP.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class BancosController : ControllerBase
{
    private readonly IBancoService _bancoService;

    public BancosController(IBancoService bancoService)
    {
        _bancoService = bancoService;
    }

    /// <summary>
    /// Lista bancos paginados
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="search">Termo de busca (opcional)</param>
    /// <returns>Lista paginada de bancos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BancoDto>>), 200)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? search = null)
    {
        var result = await _bancoService.GetPagedAsync(page, pageSize, search);
        return Ok(ApiResponse.Ok(result));
    }

    /// <summary>
    /// Lista todos os bancos (para combos/dropdowns)
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BancoDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _bancoService.GetAllAsync();
        return Ok(ApiResponse.Ok(result));
    }

    /// <summary>
    /// Obtém banco por código
    /// </summary>
    /// <param name="codigo">Código do banco (3 dígitos)</param>
    [HttpGet("{codigo}")]
    [ProducesResponseType(typeof(ApiResponse<BancoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(string codigo)
    {
        var banco = await _bancoService.GetByIdAsync(codigo);
        
        if (banco == null)
            return NotFound(ApiResponse.Fail<BancoDto>("Banco não encontrado"));
        
        return Ok(ApiResponse.Ok(banco));
    }

    /// <summary>
    /// Cria novo banco
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BancoDto>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateBancoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bancoService.CreateAsync(dto);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Code == "BANCO_DUPLICADO")
                return Conflict(ApiResponse.Fail<BancoDto>(result.Error.Message));
            
            return BadRequest(ApiResponse.Fail<BancoDto>(result.Error.Message));
        }

        return CreatedAtAction(
            nameof(GetById), 
            new { codigo = result.Value!.CodigoBanco }, 
            ApiResponse.Ok(result.Value, "Banco criado com sucesso"));
    }

    /// <summary>
    /// Atualiza banco existente
    /// </summary>
    [HttpPut("{codigo}")]
    [ProducesResponseType(typeof(ApiResponse<BancoDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string codigo, [FromBody] UpdateBancoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bancoService.UpdateAsync(codigo, dto);
        
        if (!result.IsSuccess)
        {
            if (result.Error.Code == "BANCO_NAO_ENCONTRADO")
                return NotFound(ApiResponse.Fail<BancoDto>(result.Error.Message));
            
            return BadRequest(ApiResponse.Fail<BancoDto>(result.Error.Message));
        }

        return Ok(ApiResponse.Ok(result.Value!, "Banco atualizado com sucesso"));
    }

    /// <summary>
    /// Remove banco
    /// </summary>
    [HttpDelete("{codigo}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string codigo)
    {
        var result = await _bancoService.DeleteAsync(codigo);
        
        if (!result.IsSuccess)
        {
            return result.Error.Code switch
            {
                "BANCO_NAO_ENCONTRADO" => NotFound(ApiResponse.Fail<object>(result.Error.Message)),
                "BANCO_COM_AGENCIAS" => BadRequest(ApiResponse.Fail<object>(result.Error.Message)),
                "BANCO_COM_FUNCIONARIOS" => BadRequest(ApiResponse.Fail<object>(result.Error.Message)),
                _ => BadRequest(ApiResponse.Fail<object>(result.Error.Message))
            };
        }

        return NoContent();
    }
}