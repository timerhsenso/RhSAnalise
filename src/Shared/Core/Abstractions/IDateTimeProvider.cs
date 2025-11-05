// Copyright (c) RhSenso. Todos os direitos reservados.

using System;

namespace RhSensoERP.Shared.Core.Abstractions;

/// <summary>Fornece data/hora atual (testável).</summary>
public interface IDateTimeProvider
{
    /// <summary>UTC now.</summary>
    DateTime UtcNow { get; }
}
