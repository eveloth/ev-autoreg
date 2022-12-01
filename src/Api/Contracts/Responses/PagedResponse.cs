namespace Api.Contracts.Responses;

public class PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> data, PaginationQuery pagination)
    {
        Data = data;
        PageNumber = pagination.PageNumber;
        PageSize = pagination.PageSize;
        NextPage = pagination.PageNumber + 1;
        PreviousPage = pagination.PageNumber > 1 ? pagination.PageNumber - 1 : null;
    }

    public IEnumerable<T> Data { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public int? NextPage { get; init; }
    public int? PreviousPage { get; init; }
}
