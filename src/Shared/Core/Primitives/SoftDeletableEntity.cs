// RhSensoERP.Shared.Core — SoftDeletableEntity
// Finalidade: Padrão de exclusão lógica (soft delete) com metadados.
// Uso: Herde em entidades que não devem ser removidas fisicamente do banco.
// Observações: Combine com um filtro global de consulta na Infrastructure.

using System;

namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>
/// Entidade base com suporte a exclusão lógica (soft delete).
/// </summary>
public abstract class SoftDeletableEntity
{
    /// <summary>Indica se o registro está excluído logicamente.</summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>Usuário que realizou a exclusão lógica.</summary>
    public string? DeletedBy { get; protected set; }

    /// <summary>Data/hora UTC da exclusão lógica.</summary>
    public DateTime? DeletedOn { get; protected set; }

    /// <summary>Marca a entidade como excluída logicamente.</summary>
    public void MarkAsDeleted(string? user, DateTime nowUtc)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedBy = user;
        DeletedOn = nowUtc;
    }

    /// <summary>Restaura a entidade removendo a marca de exclusão lógica.</summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedOn = null;
    }
}
