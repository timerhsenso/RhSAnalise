// Copyright (c) RhSenso. Todos os direitos reservados.

using System.Collections.Generic;

namespace RhSensoERP.Shared.Core.Common;

/// <summary>Resultado paginado padrão.</summary>
public sealed class PagedResult<T>
{
    public int PageNumber { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public IReadOnlyList<T> Items { get; }

    public PagedResult(IReadOnlyList<T> items, int pageNumber, int pageSize, long totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
