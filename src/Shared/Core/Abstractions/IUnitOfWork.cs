namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Unit of Work pattern.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Salva as alterações no banco de dados.
    /// </summary>
    /// <returns>Número de registros afetados.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
