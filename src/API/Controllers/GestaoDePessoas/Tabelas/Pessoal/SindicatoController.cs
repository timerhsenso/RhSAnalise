// src/API/Controllers/GestaoDePessoas/Tabelas/Pessoal/SindicatoController.cs

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace RhSensoERP.API.Controllers.GestaoDePessoas.Tabelas.Pessoal;

/// <summary>
/// Controller para gerenciamento de sindicatos.
/// </summary>
[ApiController]
[Route("api/v1/gestaodepessoas/tabelas/pessoal/sindicatos")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "GestaoDePessoas")]
[SwaggerTag("Cadastro de sindicatos patronais e de empregados")]
public class SindicatoController : ControllerBase
{
    private readonly ISindicatoRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<SindicatoController> _logger;

    public SindicatoController(
        ISindicatoRepository repository,
        IMapper mapper,
        ILogger<SindicatoController> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os sindicatos.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os sindicatos",
        Description = "Retorna lista completa de sindicatos ordenada por código")]
    [ProducesResponseType(typeof(List<SindicatoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SindicatoDto>>> GetAll()
    {
        var sindicatos = await _repository.GetAllAsync();
        return Ok(_mapper.Map<List<SindicatoDto>>(sindicatos));
    }

    /// <summary>
    /// Busca sindicato por código.
    /// </summary>
    [HttpGet("{codigo}")]
    [SwaggerOperation(
        Summary = "Busca sindicato por código",
        Description = "Retorna dados do sindicato pelo código (2 caracteres)")]
    [ProducesResponseType(typeof(SindicatoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SindicatoDto>> GetById(string codigo)
    {
        var sindicato = await _repository.GetByIdAsync(codigo);

        if (sindicato == null)
        {
            _logger.LogWarning("Sindicato {Codigo} não encontrado", codigo);
            return NotFound(new { message = $"Sindicato {codigo} não encontrado" });
        }

        return Ok(_mapper.Map<SindicatoDto>(sindicato));
    }

    /// <summary>
    /// Cria novo sindicato.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria novo sindicato",
        Description = "Cadastra um novo sindicato no sistema")]
    [ProducesResponseType(typeof(SindicatoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SindicatoDto>> Create([FromBody] CreateSindicatoDto dto)
    {
        // Verifica duplicação por código
        if (await _repository.ExistsAsync(dto.CodigoSindicato))
        {
            return Conflict(new { message = $"Já existe um sindicato com o código {dto.CodigoSindicato}" });
        }

        var sindicato = _mapper.Map<Sindicato>(dto);
        sindicato.Id = Guid.NewGuid();
        sindicato.CodigoSindicato = dto.CodigoSindicato.ToUpperInvariant();

        await _repository.AddAsync(sindicato);

        _logger.LogInformation(
            "Sindicato {Codigo} - {Descricao} criado",
            sindicato.CodigoSindicato,
            sindicato.DescricaoSindicato);

        return CreatedAtAction(
            nameof(GetById),
            new { codigo = sindicato.CodigoSindicato },
            _mapper.Map<SindicatoDto>(sindicato));
    }

    /// <summary>
    /// Atualiza sindicato.
    /// </summary>
    [HttpPut("{codigo}")]
    [SwaggerOperation(
        Summary = "Atualiza sindicato",
        Description = "Atualiza dados de um sindicato existente")]
    [ProducesResponseType(typeof(SindicatoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SindicatoDto>> Update(string codigo, [FromBody] UpdateSindicatoDto dto)
    {
        var sindicato = await _repository.GetByIdAsync(codigo);
        if (sindicato == null)
        {
            return NotFound(new { message = $"Sindicato {codigo} não encontrado" });
        }

        // Atualiza propriedades
        sindicato.DescricaoSindicato = dto.DescricaoSindicato.Trim();
        sindicato.Endereco = dto.Endereco?.Trim();
        sindicato.CNPJ = dto.CNPJ;
        sindicato.CodigoEntidade = dto.CodigoEntidade;
        sindicato.DataBase = dto.DataBase;
        sindicato.FlagTipo = dto.FlagTipo;
        sindicato.CodigoTabelaBase = dto.CodigoTabelaBase;

        await _repository.UpdateAsync(sindicato);

        _logger.LogInformation(
            "Sindicato {Codigo} - {Descricao} atualizado",
            codigo,
            sindicato.DescricaoSindicato);

        return Ok(_mapper.Map<SindicatoDto>(sindicato));
    }

    /// <summary>
    /// Remove sindicato.
    /// </summary>
    [HttpDelete("{codigo}")]
    [SwaggerOperation(
        Summary = "Remove sindicato",
        Description = "Exclui um sindicato do sistema")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(string codigo)
    {
        var sindicato = await _repository.GetByIdAsync(codigo);
        if (sindicato == null)
        {
            return NotFound(new { message = $"Sindicato {codigo} não encontrado" });
        }

        // Verifica se há registros dependentes
        if (sindicato.Funcionarios?.Any() == true)
        {
            _logger.LogWarning(
                "Tentativa de excluir sindicato {Codigo} com {Count} funcionários vinculados",
                codigo,
                sindicato.Funcionarios.Count);

            return Conflict(new
            {
                message = $"Não é possível excluir o sindicato {codigo}",
                reason = $"Existem {sindicato.Funcionarios.Count} funcionário(s) vinculado(s)"
            });
        }

        if (sindicato.Filiais?.Any() == true)
        {
            _logger.LogWarning(
                "Tentativa de excluir sindicato {Codigo} com {Count} filiais vinculadas",
                codigo,
                sindicato.Filiais.Count);

            return Conflict(new
            {
                message = $"Não é possível excluir o sindicato {codigo}",
                reason = $"Existem {sindicato.Filiais.Count} filial(is) vinculada(s)"
            });
        }

        await _repository.DeleteAsync(codigo);

        _logger.LogInformation(
            "Sindicato {Codigo} - {Descricao} excluído",
            codigo,
            sindicato.DescricaoSindicato);

        return NoContent();
    }
}