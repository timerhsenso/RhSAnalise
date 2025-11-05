// Copyright (c) RhSenso. Todos os direitos reservados.

using System;

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>Metadados de auditoria.</summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; }
    string? CreatedBy { get; }
    DateTime? UpdatedAtUtc { get; }
    string? UpdatedBy { get; }
}
