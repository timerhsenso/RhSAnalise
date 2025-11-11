// src/API/Controllers/GestaoDePessoas/mTabelas/Pessoal/BancosController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Application.Services;
using RhSensoERP.Shared.Core.Common;
using RhSensoERP.Shared.Contracts.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace RhSensoERP.API.Controllers.GestaoDePessoas.mTabelas.Pessoal;

/// <summary>
/// Controller para gerenciamento de bancos.
/// </summary>
[ApiController]
[Route("api/v1/gestaodepessoas/tabelas/pessoal/bancos")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "GestaoDePessoas")]
[SwaggerTag("Cadastro e manutenção de bancos")]
public class BancosController : ControllerBase
{
    private readonly IBancoService _bancoService;
    private readonly ILogger<BancosController> _logger;

    public BancosController(IBancoService bancoService, ILogger<BancosController> logger)
    {
        _bancoService = bancoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista bancos paginados.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Lista bancos paginados", Description = "Retorna lista paginada de bancos com busca opcional")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BancoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagedResult<BancoDto>>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        try
        {
            // Validação de parâmetros
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _bancoService.GetPagedAsync(page, pageSize, search);

            _logger.LogInformation(
                "Listagem de bancos retornou {Count} de {Total} registros (Página {Page})",
                result.Items.Count(),
                result.TotalCount,
                page);

            return Ok(ApiResponse.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar bancos paginados");
            return BadRequest(ApiResponse.Fail<PagedResult<BancoDto>>("Erro ao listar bancos"));
        }
    }

    /// <summary>
    /// Lista todos os bancos (para combos/dropdowns).
    /// </summary>
    [HttpGet("all")]
    [SwaggerOperation(Summary = "Lista todos os bancos", Description = "Retorna lista completa de bancos para uso em combos")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BancoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BancoDto>>>> GetAll()
    {
        try
        {
            var result = await _bancoService.GetAllAsync();
            return Ok(ApiResponse.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar todos os bancos");
            return BadRequest(ApiResponse.Fail<IEnumerable<BancoDto>>("Erro ao listar bancos"));
        }
    }

    /// <summary>
    /// Obtém banco por código.
    /// </summary>
    [HttpGet("{codigo}")]
    [SwaggerOperation(Summary = "Busca banco por código", Description = "Retorna dados do banco pelo código")]
    [ProducesResponseType(typeof(ApiResponse<BancoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BancoDto>>> GetById(string codigo)
    {
        try
        {
            var banco = await _bancoService.GetByIdAsync(codigo);

            if (banco == null)
            {
                _logger.LogWarning("Banco {Codigo} não encontrado", codigo);
                return NotFound(ApiResponse.Fail<BancoDto>($"Banco {codigo} não encontrado"));
            }

            return Ok(ApiResponse.Ok(banco));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar banco {Codigo}", codigo);
            return BadRequest(ApiResponse.Fail<BancoDto>("Erro ao buscar banco"));
        }
    }

    /// <summary>
    /// Cria novo banco.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(Summary = "Cria novo banco", Description = "Cadastra um novo banco no sistema")]
    [ProducesResponseType(typeof(ApiResponse<BancoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<BancoDto>>> Create([FromBody] CreateBancoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse.Fail<BancoDto>(string.Join("; ", errors)));
            }

            var result = await _bancoService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Falha ao criar banco: {Error}", result.Error.Message);

                if (result.Error.Code == "BANCO_DUPLICADO")
                {
                    return Conflict(ApiResponse.Fail<BancoDto>(result.Error.Message));
                }

                return BadRequest(ApiResponse.Fail<BancoDto>(result.Error.Message));
            }

            _logger.LogInformation("Banco {Codigo} criado com sucesso", result.Value!.CodigoBanco);

            return CreatedAtAction(
                nameof(GetById),
                new { codigo = result.Value!.CodigoBanco },
                ApiResponse.Ok(result.Value, "Banco criado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar banco");
            return BadRequest(ApiResponse.Fail<BancoDto>("Erro ao criar banco"));
        }
    }

    /// <summary>
    /// Atualiza banco existente.
    /// </summary>
    [HttpPut("{codigo}")]
    [SwaggerOperation(Summary = "Atualiza banco", Description = "Atualiza dados de um banco existente")]
    [ProducesResponseType(typeof(ApiResponse<BancoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BancoDto>>> Update(string codigo, [FromBody] UpdateBancoDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .SelectMany(x => x.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
                    .Select(x => x.ErrorMessage)
                    .ToList();

                return BadRequest(ApiResponse.Fail<BancoDto>(string.Join("; ", errors)));
            }

            var result = await _bancoService.UpdateAsync(codigo, dto);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Falha ao atualizar banco {Codigo}: {Error}", codigo, result.Error.Message);

                if (result.Error.Code == "BANCO_NAO_ENCONTRADO")
                {
                    return NotFound(ApiResponse.Fail<BancoDto>(result.Error.Message));
                }

                return BadRequest(ApiResponse.Fail<BancoDto>(result.Error.Message));
            }

            _logger.LogInformation("Banco {Codigo} atualizado com sucesso", codigo);

            return Ok(ApiResponse.Ok(result.Value!, "Banco atualizado com sucesso"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar banco {Codigo}", codigo);
            return BadRequest(ApiResponse.Fail<BancoDto>("Erro ao atualizar banco"));
        }
    }

    /// <summary>
    /// Remove banco.
    /// </summary>
    [HttpDelete("{codigo}")]
    [SwaggerOperation(Summary = "Remove banco", Description = "Exclui um banco do sistema")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string codigo)
    {
        try
        {
            var result = await _bancoService.DeleteAsync(codigo);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Falha ao excluir banco {Codigo}: {Error}", codigo, result.Error.Message);

                return result.Error.Code switch
                {
                    "BANCO_NAO_ENCONTRADO" => NotFound(ApiResponse.Fail(result.Error.Message)),
                    "BANCO_COM_AGENCIAS" => BadRequest(ApiResponse.Fail(result.Error.Message)),
                    "BANCO_COM_FUNCIONARIOS" => BadRequest(ApiResponse.Fail(result.Error.Message)),
                    _ => BadRequest(ApiResponse.Fail(result.Error.Message))
                };
            }

            _logger.LogInformation("Banco {Codigo} excluído com sucesso", codigo);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir banco {Codigo}", codigo);
            return BadRequest(ApiResponse.Fail("Erro ao excluir banco"));
        }
    }
}