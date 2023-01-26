using Api.Errors;

namespace Api.Exceptions;

public static class Thrower
{
    public static ApiException WithApiError(this ApiException e, ApiError error)
    {
        e.Data.Add(nameof(ApiError), error);
        return e;
    }
}