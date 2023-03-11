namespace EvAutoreg.Api.Contracts.Queries;

public record PaginationQuery
{
    private readonly int _pageSize;
    private readonly int _pageNumber;

    public int PageNumber
    {
        get => _pageNumber;
        init
        {
            _pageNumber = value switch
            {
                < 1 => 1,
                _ => value
            };
        }
    }

    public int PageSize
    {
        get => _pageSize;
        init
        {
            _pageSize = value switch
            {
                > 100 => 100,
                < 1 => 1,
                _ => value
            };
        }
    }

    public PaginationQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public PaginationQuery() : this(1, 50) { }
}