namespace Api.Contracts;

public record PaginationQuery
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    private PaginationQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize > 100 ? 100 : pageSize;
    }

    public PaginationQuery() : this(1, 50) { }
}
