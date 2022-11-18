using DataAccessLibrary.Filters;

namespace EvAutoreg.Contracts.Extensions;

public static class ContractExtensions
{
    public static PaginationFilter ToFilter(this PaginationQuery query)
    {
        return new PaginationFilter(query.PageNumber, query.PageSize);
    }
}
