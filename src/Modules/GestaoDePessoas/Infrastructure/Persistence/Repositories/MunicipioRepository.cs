// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Repositories/MunicipioRepository.cs

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Repositories;

/// <summary>
/// Interface para repositório de municípios.
/// </summary>
public interface IMunicipioRepository
{
    Task<List<Municipio>> GetAllAsync();
    Task<List<Municipio>> GetByEstadoAsync(string uf);
    Task<Municipio?> GetByIdAsync(string codigo);
    Task<bool> ExistsAsync(string codigo);
    Task<bool> ExistsByNomeEstadoAsync(string nome, string uf, string? excludeCodigo = null);
    Task<Municipio> AddAsync(Municipio municipio);
    Task<Municipio> UpdateAsync(Municipio municipio);
    Task DeleteAsync(string codigo);
}

/// <summary>
/// Implementação do repositório de municípios.
/// </summary>
public sealed class MunicipioRepository : IMunicipioRepository
{
    private readonly GestaoDePessoasDbContext _context;

    public MunicipioRepository(GestaoDePessoasDbContext context)
    {
        _context = context;
    }

    public async Task<List<Municipio>> GetAllAsync()
    {
        return await _context.Municipios
            .AsNoTracking()
            .OrderBy(m => m.SiglaEstado)
            .ThenBy(m => m.NomeMunicipio)
            .ToListAsync();
    }

    public async Task<List<Municipio>> GetByEstadoAsync(string uf)
    {
        return await _context.Municipios
            .AsNoTracking()
            .Where(m => m.SiglaEstado == uf.ToUpperInvariant())
            .OrderBy(m => m.NomeMunicipio)
            .ToListAsync();
    }

    public async Task<Municipio?> GetByIdAsync(string codigo)
    {
        return await _context.Municipios
            .FirstOrDefaultAsync(m => m.CodigoMunicipio == codigo);
    }

    public async Task<bool> ExistsAsync(string codigo)
    {
        return await _context.Municipios
            .AnyAsync(m => m.CodigoMunicipio == codigo);
    }

    public async Task<bool> ExistsByNomeEstadoAsync(string nome, string uf, string? excludeCodigo = null)
    {
        var query = _context.Municipios
            .Where(m => m.NomeMunicipio.ToUpper() == nome.ToUpper() &&
                        m.SiglaEstado == uf.ToUpperInvariant());

        if (!string.IsNullOrWhiteSpace(excludeCodigo))
        {
            query = query.Where(m => m.CodigoMunicipio != excludeCodigo);
        }

        return await query.AnyAsync();
    }

    public async Task<Municipio> AddAsync(Municipio municipio)
    {
        _context.Municipios.Add(municipio);
        await _context.SaveChangesAsync();
        return municipio;
    }

    public async Task<Municipio> UpdateAsync(Municipio municipio)
    {
        _context.Municipios.Update(municipio);
        await _context.SaveChangesAsync();
        return municipio;
    }

    public async Task DeleteAsync(string codigo)
    {
        var municipio = await GetByIdAsync(codigo);
        if (municipio != null)
        {
            _context.Municipios.Remove(municipio);
            await _context.SaveChangesAsync();
        }
    }
}