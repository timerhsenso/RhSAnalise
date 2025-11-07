using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Core.Interfaces.Repositories;

public interface ISistemaRepository : IRepository<Sistema>
{
    Task<Sistema?> GetByIdAsync(string cdSistema, CancellationToken ct = default);
    Task<bool> ExistsAsync(string cdSistema, CancellationToken ct = default);
    Task<(IReadOnlyList<Sistema> Items, int TotalCount)> ListPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    IUnitOfWork UnitOfWork { get; }
}
