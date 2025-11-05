namespace RhSensoERP.Shared.Contracts.Common;

public sealed class PagedRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SortBy { get; init; }

    public bool Desc { get; init; }
}
