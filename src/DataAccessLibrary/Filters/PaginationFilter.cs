namespace DataAccessLibrary.Filters;

public readonly record struct PaginationFilter(int? PageNumber, int? Pagesize);
