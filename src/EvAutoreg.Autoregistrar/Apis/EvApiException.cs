using System.Net;

namespace EvAutoreg.Autoregistrar.Apis;

public class EvApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public EvApiException() { }

    public EvApiException(string message) : base(message) { }

    public EvApiException(string message, Exception inner) : base(message, inner) { }

    public EvApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public override string ToString()
    {
        return StatusCode is null
            ? base.ToString()
            : $"Request cannot be executed: StatusCode: {StatusCode} EV server response: {Message}";
    }
}