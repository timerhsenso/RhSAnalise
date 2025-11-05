// Copyright (c) RhSenso. Todos os direitos reservados.

using System;

namespace RhSensoERP.Shared.Core.Primitives;

/// <summary>Entidade base com Id e metadados comuns.</summary>
public abstract class BaseEntity
{
    /// <summary>Identificador único.</summary>
    public Guid Id { get; protected set; }

    /// <summary>Data/hora de criação (UTC).</summary>
    public DateTime CreatedAtUtc { get; protected set; }

    /// <summary>Usuário que criou.</summary>
    public string? CreatedBy { get; protected set; }

    /// <summary>Data/hora da última atualização (UTC).</summary>
    public DateTime? UpdatedAtUtc { get; protected set; }

    /// <summary>Usuário que atualizou.</summary>
    public string? UpdatedBy { get; protected set; }

    /// <summary>Construtor padrão inicializa o Id.</summary>
    protected BaseEntity() => Id = Guid.NewGuid();
}
