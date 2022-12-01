namespace Api.Contracts.Responses;

public record Response<T>(T Data)
{
    public T Data { get; set; } = Data;
}
