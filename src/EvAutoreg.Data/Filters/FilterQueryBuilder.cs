using System.Text;
using Microsoft.Extensions.Primitives;

namespace EvAutoreg.Data.Filters;

public class FilterQueryBuilder : IFilterQueryBuilder
{
    private const string IncludeDeletedSql = "is_deleted = false";

    public void ApplyPaginationFilter(
        StringBuilder queryBuilder,
        PaginationFilter? filter,
        string? orderBy = default
    )
    {
        if (filter is null)
            return;

        var skip = (filter.PageNumber - 1) * filter.PageSize;
        var take = filter.PageSize;

        if (orderBy is not null)
        {
            queryBuilder.Append(' ').Append($"ORDER BY {orderBy}");
        }

        var paginator = $"LIMIT {take} OFFSET {skip}";

        queryBuilder.Append(' ').Append(paginator);
    }

    public void ApplyIncludeDeletedFilter(
        StringBuilder queryBuilder,
        bool includeDeleted,
        ChainOptions options
    )
    {
        if (!includeDeleted)
            return;

        var chaining = options switch
        {
            ChainOptions.Where => "WHERE",
            ChainOptions.And => "AND",
            _ => throw new ArgumentOutOfRangeException(nameof(options), options, null)
        };

        queryBuilder.Append(' ').Append(chaining).Append(' ').Append(IncludeDeletedSql);
    }
}