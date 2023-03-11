using System.Text;

namespace EvAutoreg.Data.Filters;

public interface IFilterQueryBuilder
{
    void ApplyPaginationFilter(
        StringBuilder queryBuilder,
        PaginationFilter? filter,
        string? orderBy = default
    );
    void ApplyIncludeDeletedFilter(
        StringBuilder queryBuilder,
        bool includeDeleted,
        ChainOptions options
    );
}