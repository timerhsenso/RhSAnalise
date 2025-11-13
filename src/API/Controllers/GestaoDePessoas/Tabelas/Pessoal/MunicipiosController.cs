// src/API/Controllers/GestaoDePessoas/Tabelas/Pessoal/MunicipiosController.cs

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace RhSensoERP.API.Controllers.GestaoDePessoas.Tabelas.Pessoal;

/// <summary>
/// Controller para gerenciamento de municípios.
/// </summary>
[ApiController]
[Route("api/v1/gestaodepessoas/tabelas/pessoal/municipios")]
[Authorize]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "GestaoDePessoas")]
[SwaggerTag("Cadastro de municípios do Brasil")]
public class MunicipiosController : ControllerBase
{
    private readonly IMunicipioRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<MunicipiosController> _logger;

    public MunicipiosController(
        IMunicipioRepository repository,
        IMapper mapper,
        ILogger<MunicipiosController> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os municípios.
    /// </summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os municípios",
        Description = "Retorna lista completa de municípios ordenada por UF e nome")]
    [ProducesResponseType(typeof(List<MunicipioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MunicipioDto>>> GetAll()
    {
        var municipios = await _repository.GetAllAsync();
        return Ok(_mapper.Map<List<MunicipioDto>>(municipios));
    }

    /// <summary>
    /// Lista municípios por estado.
    /// </summary>
    [HttpGet("estado/{uf}")]
    [SwaggerOperation(
        Summary = "Lista municípios por UF",
        Description = "Retorna lista de municípios de um estado específico")]
    [ProducesResponseType(typeof(List<MunicipioDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MunicipioDto>>> GetByEstado(string uf)
    {
        var municipios = await _repository.GetByEstadoAsync(uf);
        return Ok(_mapper.Map<List<MunicipioDto>>(municipios));
    }

    /// <summary>
    /// Busca município por código.
    /// </summary>
    [HttpGet("{codigo}")]
    [SwaggerOperation(
        Summary = "Busca município por código",
        Description = "Retorna dados do município pelo código (5 dígitos)")]
    [ProducesResponseType(typeof(MunicipioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MunicipioDto>> GetById(string codigo)
    {
        var municipio = await _repository.GetByIdAsync(codigo);

        if (municipio == null)
        {
            _logger.LogWarning("Município {Codigo} não encontrado", codigo);
            return NotFound(new { message = $"Município {codigo} não encontrado" });
        }

        return Ok(_mapper.Map<MunicipioDto>(municipio));
    }

    /// <summary>
    /// Cria novo município.
    /// </summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria novo município",
        Description = "Cadastra um novo município no sistema")]
    [ProducesResponseType(typeof(MunicipioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MunicipioDto>> Create([FromBody] MunicipioDto dto)
    {
        // Verifica duplicação por código
        if (await _repository.ExistsAsync(dto.CodigoMunicipio))
        {
            return Conflict(new { message = $"Já existe um município com o código {dto.CodigoMunicipio}" });
        }

        // Verifica duplicação por nome e estado
        if (await _repository.ExistsByNomeEstadoAsync(dto.NomeMunicipio, dto.SiglaEstado))
        {
            return Conflict(new { message = $"Já existe o município '{dto.NomeMunicipio}' no estado {dto.SiglaEstado}" });
        }

        var municipio = _mapper.Map<Municipio>(dto);
        municipio.Id = Guid.NewGuid();
        municipio.CodigoMunicipio = dto.CodigoMunicipio.ToUpperInvariant();
        municipio.SiglaEstado = dto.SiglaEstado.ToUpperInvariant();

        await _repository.AddAsync(municipio);

        _logger.LogInformation(
            "Município {Codigo} - {Nome}/{UF} criado",
            municipio.CodigoMunicipio,
            municipio.NomeMunicipio,
            municipio.SiglaEstado);

        return CreatedAtAction(
            nameof(GetById),
            new { codigo = municipio.CodigoMunicipio },
            _mapper.Map<MunicipioDto>(municipio));
    }

    /// <summary>
    /// Atualiza município.
    /// </summary>
    [HttpPut("{codigo}")]
    [SwaggerOperation(
        Summary = "Atualiza município",
        Description = "Atualiza dados de um município existente")]
    [ProducesResponseType(typeof(MunicipioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MunicipioDto>> Update(string codigo, [FromBody] MunicipioDto dto)
    {
        var municipio = await _repository.GetByIdAsync(codigo);
        if (municipio == null)
        {
            return NotFound(new { message = $"Município {codigo} não encontrado" });
        }

        // Verifica duplicação por nome e estado (excluindo o próprio)
        if (await _repository.ExistsByNomeEstadoAsync(dto.NomeMunicipio, dto.SiglaEstado, codigo))
        {
            return Conflict(new { message = $"Já existe outro município '{dto.NomeMunicipio}' no estado {dto.SiglaEstado}" });
        }

        // Atualiza propriedades
        municipio.SiglaEstado = dto.SiglaEstado.ToUpperInvariant();
        municipio.NomeMunicipio = dto.NomeMunicipio.Trim();
        municipio.CodigoIBGE = dto.CodigoIBGE;

        await _repository.UpdateAsync(municipio);

        _logger.LogInformation(
            "Município {Codigo} - {Nome}/{UF} atualizado",
            codigo,
            municipio.NomeMunicipio,
            municipio.SiglaEstado);

        return Ok(_mapper.Map<MunicipioDto>(municipio));
    }

    /// <summary>
    /// Remove município.
    /// </summary>
    [HttpDelete("{codigo}")]
    [SwaggerOperation(
        Summary = "Remove município",
        Description = "Exclui um município do sistema")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string codigo)
    {
        var municipio = await _repository.GetByIdAsync(codigo);
        if (municipio == null)
        {
            return NotFound(new { message = $"Município {codigo} não encontrado" });
        }

        await _repository.DeleteAsync(codigo);

        _logger.LogInformation(
            "Município {Codigo} - {Nome}/{UF} excluído",
            codigo,
            municipio.NomeMunicipio,
            municipio.SiglaEstado);

        return NoContent();
    }
}