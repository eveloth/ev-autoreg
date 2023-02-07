namespace EvAutoreg.Data.Filters;

public readonly record struct PaginationFilter(int? PageNumber, int? PageSize);