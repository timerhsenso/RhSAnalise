using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Core.Interfaces.Repositories;

/// <summary>
/// Contrato do repositório de Sistema.
/// Herda operações básicas do repositório genérico e adiciona filtros específicos.
/// </summary>
public interface ISistemaRepository : IRepository<Sistema>
{
    IUnitOfWork UnitOfWork { get; }

    Task<Sistema?> GetByIdAsync(string cdSistema, CancellationToken ct = default);
    Task<bool> ExistsAsync(string cdSistema, CancellationToken ct = default);

    Task<(IReadOnlyList<Sistema> Items, int TotalCount)> ListPagedAsync(
        int page, int pageSize, string? search, CancellationToken ct = default);

    /// <summary>
    /// Mantido por compatibilidade com código existente.
    /// </summary>
    void Delete(Sistema entity);
}
