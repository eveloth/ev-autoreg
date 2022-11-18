namespace EvAutoreg.Contracts;

public class PagedResponse<T>
{
    public PagedResponse() { }

    public PagedResponse(IEnumerable<T> data)
    {
        Data = data;
    }

    public IEnumerable<T> Data { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
    public string? NextPage { get; init; }
    public string? PreviousPage { get; init; }
}
