using System.Net;

namespace Autoregistrar.Apis;

public class EvResponse<T>
{
    public required HttpStatusCode StatusCode { get; init; }
    public required T Content { get; set; }
}