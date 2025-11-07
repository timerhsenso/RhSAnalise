using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Infrastructure.Repositories;

public sealed class SistemaRepository : ISistemaRepository
{
    private readonly IdentityDbContext _ctx;

    public SistemaRepository(IdentityDbContext ctx) => _ctx = ctx;

    public IUnitOfWork UnitOfWork => _ctx;

    public async Task AddAsync(Sistema entity, CancellationToken ct = default) => await _ctx.Sistemas.AddAsync(entity, ct);

    public void Update(Sistema entity) => _ctx.Sistemas.Update(entity);

    public void Delete(Sistema entity) => _ctx.Sistemas.Remove(entity);

    public async Task<Sistema?> GetByIdAsync(string cdSistema, CancellationToken ct = default) =>
        await _ctx.Sistemas.AsNoTracking().FirstOrDefaultAsync(x => x.CdSistema == cdSistema, ct);

    public async Task<bool> ExistsAsync(string cdSistema, CancellationToken ct = default) =>
        await _ctx.Sistemas.AnyAsync(x => x.CdSistema == cdSistema, ct);

    public async Task<(IReadOnlyList<Sistema> Items, int TotalCount)> ListPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _ctx.Sistemas.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(x => x.CdSistema.Contains(s) || x.DcSistema.Contains(s));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.CdSistema)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}
