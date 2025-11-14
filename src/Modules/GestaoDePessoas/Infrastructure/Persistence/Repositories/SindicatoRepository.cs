// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Repositories/SindicatoRepository.cs

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Contexts;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Repositories;

public interface ISindicatoRepository
{
    Task<IEnumerable<Sindicato>> GetAllAsync();
    Task<Sindicato?> GetByIdAsync(string codigo);
    Task AddAsync(Sindicato sindicato);
    Task UpdateAsync(Sindicato sindicato);
    Task DeleteAsync(string codigo);
    Task<bool> ExistsAsync(string codigo);
}

public class SindicatoRepository : ISindicatoRepository
{
    private readonly GestaoDePessoasDbContext _context;

    public SindicatoRepository(GestaoDePessoasDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sindicato>> GetAllAsync()
    {
        return await _context.Sindicatos
            .AsNoTracking()
            .OrderBy(s => s.CodigoSindicato)
            .ToListAsync();
    }

    public async Task<Sindicato?> GetByIdAsync(string codigo)
    {
        return await _context.Sindicatos
            .Include(s => s.Funcionarios)
            .Include(s => s.Filiais)
            .FirstOrDefaultAsync(s => s.CodigoSindicato == codigo);
    }

    public async Task AddAsync(Sindicato sindicato)
    {
        await _context.Sindicatos.AddAsync(sindicato);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Sindicato sindicato)
    {
        _context.Sindicatos.Update(sindicato);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string codigo)
    {
        var sindicato = await _context.Sindicatos
            .FirstOrDefaultAsync(s => s.CodigoSindicato == codigo);

        if (sindicato != null)
        {
            _context.Sindicatos.Remove(sindicato);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(string codigo)
    {
        return await _context.Sindicatos
            .AnyAsync(s => s.CodigoSindicato == codigo);
    }
}