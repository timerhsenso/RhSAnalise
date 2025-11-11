using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence;
using RhSensoERP.Shared.Core.Common;

namespace RhSensoERP.Modules.GestaoDePessoas.Application.Services;

public interface IBancoService
{
    Task<PagedResult<BancoDto>> GetPagedAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<BancoDto?> GetByIdAsync(string codigoBanco);
    Task<IEnumerable<BancoDto>> GetAllAsync();
    Task<Result<BancoDto>> CreateAsync(CreateBancoDto dto);
    Task<Result<BancoDto>> UpdateAsync(string codigoBanco, UpdateBancoDto dto);
    Task<Result> DeleteAsync(string codigoBanco);
}

public class BancoService : IBancoService
{
    private readonly GestaoDePessoasContext _context;
    private readonly ILogger<BancoService> _logger;

    public BancoService(GestaoDePessoasContext context, ILogger<BancoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<BancoDto>> GetPagedAsync(int page = 1, int pageSize = 10, string? search = null)
    {
        var query = _context.Bancos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => 
                b.CodigoBanco.Contains(search) || 
                b.DescricaoBanco.Contains(search));
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(b => b.CodigoBanco)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BancoDto
            {
                CodigoBanco = b.CodigoBanco,
                DescricaoBanco = b.DescricaoBanco,
                TotalAgencias = b.Agencias.Count
            })
            .ToListAsync();

        return new PagedResult<BancoDto>(items, totalCount, page, pageSize);
    }

    public async Task<BancoDto?> GetByIdAsync(string codigoBanco)
    {
        var banco = await _context.Bancos
            .Include(b => b.Agencias)
            .FirstOrDefaultAsync(b => b.CodigoBanco == codigoBanco);

        if (banco == null)
            return null;

        return new BancoDto
        {
            CodigoBanco = banco.CodigoBanco,
            DescricaoBanco = banco.DescricaoBanco,
            TotalAgencias = banco.Agencias.Count
        };
    }

    public async Task<IEnumerable<BancoDto>> GetAllAsync()
    {
        return await _context.Bancos
            .OrderBy(b => b.DescricaoBanco)
            .Select(b => new BancoDto
            {
                CodigoBanco = b.CodigoBanco,
                DescricaoBanco = b.DescricaoBanco,
                TotalAgencias = b.Agencias.Count
            })
            .ToListAsync();
    }

    public async Task<Result<BancoDto>> CreateAsync(CreateBancoDto dto)
    {
        // Verificar se já existe
        var exists = await _context.Bancos
            .AnyAsync(b => b.CodigoBanco == dto.CodigoBanco);

        if (exists)
        {
            _logger.LogWarning("Tentativa de criar banco duplicado: {CodigoBanco}", dto.CodigoBanco);
            return Result<BancoDto>.Failure("BANCO_DUPLICADO", $"Banco {dto.CodigoBanco} já está cadastrado");
        }

        var banco = new Banco
        {
            CodigoBanco = dto.CodigoBanco,
            DescricaoBanco = dto.DescricaoBanco
        };

        _context.Bancos.Add(banco);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banco {CodigoBanco} - {Descricao} criado com sucesso", 
            banco.CodigoBanco, banco.DescricaoBanco);

        return Result<BancoDto>.Success(new BancoDto
        {
            CodigoBanco = banco.CodigoBanco,
            DescricaoBanco = banco.DescricaoBanco,
            TotalAgencias = 0
        });
    }

    public async Task<Result<BancoDto>> UpdateAsync(string codigoBanco, UpdateBancoDto dto)
    {
        var banco = await _context.Bancos
            .Include(b => b.Agencias)
            .FirstOrDefaultAsync(b => b.CodigoBanco == codigoBanco);

        if (banco == null)
        {
            _logger.LogWarning("Banco {CodigoBanco} não encontrado para atualização", codigoBanco);
            return Result<BancoDto>.Failure("BANCO_NAO_ENCONTRADO", "Banco não encontrado");
        }

        banco.DescricaoBanco = dto.DescricaoBanco;
        
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banco {CodigoBanco} atualizado", codigoBanco);

        return Result<BancoDto>.Success(new BancoDto
        {
            CodigoBanco = banco.CodigoBanco,
            DescricaoBanco = banco.DescricaoBanco,
            TotalAgencias = banco.Agencias.Count
        });
    }

    public async Task<Result> DeleteAsync(string codigoBanco)
    {
        var banco = await _context.Bancos
            .Include(b => b.Agencias)
            .Include(b => b.Funcionarios)
            .FirstOrDefaultAsync(b => b.CodigoBanco == codigoBanco);

        if (banco == null)
        {
            _logger.LogWarning("Banco {CodigoBanco} não encontrado para exclusão", codigoBanco);
            return Result.Failure(new Error("BANCO_NAO_ENCONTRADO", "Banco não encontrado"));
        }

        // Verificar integridade referencial
        if (banco.Agencias.Any())
        {
            _logger.LogWarning("Tentativa de excluir banco {CodigoBanco} com {Count} agências", 
                codigoBanco, banco.Agencias.Count);
            return Result.Failure(new Error("BANCO_COM_AGENCIAS", 
                $"Banco possui {banco.Agencias.Count} agência(s) cadastrada(s)"));
        }

        if (banco.Funcionarios.Any())
        {
            _logger.LogWarning("Tentativa de excluir banco {CodigoBanco} com funcionários vinculados", codigoBanco);
            return Result.Failure(new Error("BANCO_COM_FUNCIONARIOS", 
                "Existem funcionários vinculados a este banco"));
        }

        _context.Bancos.Remove(banco);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Banco {CodigoBanco} excluído com sucesso", codigoBanco);

        return Result.Success();
    }
}