using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.Esocial.Infrastructure.Persistence.Contexts;
using RhSensoERP.Shared.Contracts.Esocial.DTOs;
using RhSensoERP.Shared.Contracts.Esocial.Interfaces;

namespace RhSensoERP.Modules.Esocial.Infrastructure.Services;

public sealed class EsocialLookupService : IEsocialLookupService
{
    private readonly EsocialDbContext _context;

    public EsocialLookupService(EsocialDbContext context)
    {
        _context = context;
    }

    // Tabela 4
    public async Task<IReadOnlyList<Tabela4LookupDto>> GetTabela4Async(CancellationToken ct = default)
    {
        return await _context.Tabela4
            .AsNoTracking()
            .OrderBy(t => t.Tab4Codigo)
            .Select(t => new Tabela4LookupDto(t.Tab4Codigo, t.Tab4Descricao))
            .ToListAsync(ct);
    }

    public async Task<Tabela4LookupDto?> GetTabela4ByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Tabela4
            .AsNoTracking()
            .Where(t => t.Tab4Codigo == codigo)
            .Select(t => new Tabela4LookupDto(t.Tab4Codigo, t.Tab4Descricao))
            .FirstOrDefaultAsync(ct);
    }

    // Tabela 8
    public async Task<IReadOnlyList<Tabela8LookupDto>> GetTabela8Async(CancellationToken ct = default)
    {
        return await _context.Tabela8
            .AsNoTracking()
            .OrderBy(t => t.Tab8Codigo)
            .Select(t => new Tabela8LookupDto(t.Tab8Codigo, t.Tab8Descricao))
            .ToListAsync(ct);
    }

    public async Task<Tabela8LookupDto?> GetTabela8ByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Tabela8
            .AsNoTracking()
            .Where(t => t.Tab8Codigo == codigo)
            .Select(t => new Tabela8LookupDto(t.Tab8Codigo, t.Tab8Descricao))
            .FirstOrDefaultAsync(ct);
    }

    // Tabela 10
    public async Task<IReadOnlyList<Tabela10LookupDto>> GetTabela10Async(CancellationToken ct = default)
    {
        return await _context.Tabela10
            .AsNoTracking()
            .OrderBy(t => t.Tab10Codigo)
            .Select(t => new Tabela10LookupDto(t.Tab10Codigo, t.Tab10Descricao))
            .ToListAsync(ct);
    }

    public async Task<Tabela10LookupDto?> GetTabela10ByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Tabela10
            .AsNoTracking()
            .Where(t => t.Tab10Codigo == codigo)
            .Select(t => new Tabela10LookupDto(t.Tab10Codigo, t.Tab10Descricao))
            .FirstOrDefaultAsync(ct);
    }

    // Tabela 21
    public async Task<IReadOnlyList<Tabela21LookupDto>> GetTabela21Async(CancellationToken ct = default)
    {
        return await _context.Tabela21
            .AsNoTracking()
            .OrderBy(t => t.Tab21Codigo)
            .Select(t => new Tabela21LookupDto(t.Tab21Codigo, t.Tab21Descricao))
            .ToListAsync(ct);
    }

    public async Task<Tabela21LookupDto?> GetTabela21ByCodigoAsync(string codigo, CancellationToken ct = default)
    {
        return await _context.Tabela21
            .AsNoTracking()
            .Where(t => t.Tab21Codigo == codigo)
            .Select(t => new Tabela21LookupDto(t.Tab21Codigo, t.Tab21Descricao))
            .FirstOrDefaultAsync(ct);
    }

    // Lotação Tributária
    public async Task<IReadOnlyList<LotacaoTributariaLookupDto>> GetLotacoesTributariasAsync(CancellationToken ct = default)
    {
        return await _context.LotacoesTributarias
            .Include(l => l.Tabela10)
            .Include(l => l.Tabela4)
            .AsNoTracking()
            .OrderBy(l => l.CodLotacao)
            .Select(l => new LotacaoTributariaLookupDto(
                l.Id,
                l.CodLotacao,
                l.Descricao,
                l.TpLotacao,
                l.Tabela10 != null ? l.Tabela10.Tab10Descricao : string.Empty,
                l.Fpas,
                l.Tabela4 != null ? l.Tabela4.Tab4Descricao : string.Empty))
            .ToListAsync(ct);
    }

    public async Task<LotacaoTributariaLookupDto?> GetLotacaoTributariaByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.LotacoesTributarias
            .Include(l => l.Tabela10)
            .Include(l => l.Tabela4)
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LotacaoTributariaLookupDto(
                l.Id,
                l.CodLotacao,
                l.Descricao,
                l.TpLotacao,
                l.Tabela10 != null ? l.Tabela10.Tab10Descricao : string.Empty,
                l.Fpas,
                l.Tabela4 != null ? l.Tabela4.Tab4Descricao : string.Empty))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<LotacaoTributariaLookupDto?> GetLotacaoTributariaByCodLotacaoAsync(string codLotacao, CancellationToken ct = default)
    {
        return await _context.LotacoesTributarias
            .Include(l => l.Tabela10)
            .Include(l => l.Tabela4)
            .AsNoTracking()
            .Where(l => l.CodLotacao == codLotacao)
            .Select(l => new LotacaoTributariaLookupDto(
                l.Id,
                l.CodLotacao,
                l.Descricao,
                l.TpLotacao,
                l.Tabela10 != null ? l.Tabela10.Tab10Descricao : string.Empty,
                l.Fpas,
                l.Tabela4 != null ? l.Tabela4.Tab4Descricao : string.Empty))
            .FirstOrDefaultAsync(ct);
    }

    // Motivo de Ocorrência
    public async Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosOcorrenciaAsync(int? tipoOcorrencia = null, CancellationToken ct = default)
    {
        var query = _context.MotivosOcorrencia.AsNoTracking();

        if (tipoOcorrencia.HasValue)
            query = query.Where(m => m.TpOcorr == tipoOcorrencia.Value);

        return await query
            .OrderBy(m => m.TpOcorr)
            .ThenBy(m => m.DcMotoc)
            .Select(m => new MotivoOcorrenciaLookupDto(
                m.Id,
                m.CdMotoc,
                m.TpOcorr,
                m.DcMotoc,
                GetTipoOcorrenciaDescricao(m.TpOcorr)))
            .ToListAsync(ct);
    }

    public async Task<MotivoOcorrenciaLookupDto?> GetMotivoOcorrenciaByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.MotivosOcorrencia
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new MotivoOcorrenciaLookupDto(
                m.Id,
                m.CdMotoc,
                m.TpOcorr,
                m.DcMotoc,
                GetTipoOcorrenciaDescricao(m.TpOcorr)))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<MotivoOcorrenciaLookupDto?> GetMotivoOcorrenciaByCodigoAsync(string cdMotoc, int tpOcorr, CancellationToken ct = default)
    {
        return await _context.MotivosOcorrencia
            .AsNoTracking()
            .Where(m => m.CdMotoc == cdMotoc && m.TpOcorr == tpOcorr)
            .Select(m => new MotivoOcorrenciaLookupDto(
                m.Id,
                m.CdMotoc,
                m.TpOcorr,
                m.DcMotoc,
                GetTipoOcorrenciaDescricao(m.TpOcorr)))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosHoraExtraAsync(CancellationToken ct = default)
        => await GetMotivosOcorrenciaAsync(tipoOcorrencia: 2, ct);

    public async Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosFaltaAsync(CancellationToken ct = default)
        => await GetMotivosOcorrenciaAsync(tipoOcorrencia: 1, ct);

    public async Task<IReadOnlyList<MotivoOcorrenciaLookupDto>> GetMotivosAtrasoAsync(CancellationToken ct = default)
        => await GetMotivosOcorrenciaAsync(tipoOcorrencia: 3, ct);

    private static string GetTipoOcorrenciaDescricao(int tpOcorr)
    {
        return tpOcorr switch
        {
            1 => "Falta",
            2 => "Hora Extra",
            3 => "Atraso",
            4 => "Afastamento",
            5 => "Abono",
            _ => $"Tipo {tpOcorr}"
        };
    }
}