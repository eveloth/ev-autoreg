namespace EvAutoreg.Api.Contracts.Responses;

public record PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> data, PaginationQuery pagination, int entitesCount)
    {
        Data = data;
        PageNumber = pagination.PageNumber;
        PageSize = pagination.PageSize;
        Total = entitesCount;
    }

    public IEnumerable<T> Data { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int Total { get; }
    public int PagesTotal => (int)Math.Ceiling((double)Total / PageSize);
}