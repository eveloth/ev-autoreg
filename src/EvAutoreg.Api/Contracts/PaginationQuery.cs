namespace EvAutoreg.Api.Contracts;

public record PaginationQuery
{
    private int _pageSize;
    public int PageNumber { get; init; }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > 100 ? 100 : value;
    }

    public PaginationQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public PaginationQuery() : this(1, 50) { }
}