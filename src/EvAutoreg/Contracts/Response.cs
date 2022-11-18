namespace EvAutoreg.Contracts;

public record Response<T>(IEnumerable<T> Data)
{
    public IEnumerable<T> Data { get; set; } = Data;
}
