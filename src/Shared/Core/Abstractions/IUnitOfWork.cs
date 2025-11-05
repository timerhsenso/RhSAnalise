// Copyright (c) RhSenso. Todos os direitos reservados.

using System.Threading;
using System.Threading.Tasks;

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>Unidade de trabalho para persistência transacional.</summary>
public interface IUnitOfWork
{
    /// <summary>Salva mudanças pendentes.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
