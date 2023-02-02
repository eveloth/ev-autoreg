using System.Net;

namespace EvAutoreg.Autoregistrar.Apis;

public class EvApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public EvApiException() { }

    public EvApiException(string message) : base(message) { }

    public EvApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public EvApiException(string message, Exception inner) : base(message, inner) { }

    public EvApiException(string message, Exception inner, HttpStatusCode statusCode)
        : base(message, inner)
    {
        StatusCode = statusCode;
    }
}