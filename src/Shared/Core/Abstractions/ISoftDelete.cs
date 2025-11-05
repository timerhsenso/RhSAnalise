// Copyright (c) RhSenso. Todos os direitos reservados.

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>Marca entidades que suportam soft delete.</summary>
public interface ISoftDelete
{
    /// <summary>Indica se o registro está logicamente excluído.</summary>
    bool IsDeleted { get; }
}
