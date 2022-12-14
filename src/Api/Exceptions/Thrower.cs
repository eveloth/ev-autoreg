using Api.Errors;

namespace Api.Exceptions;

public static class Thrower
{
    public static void ThrowApiException(ApiError error)
    {
        var e = new ApiException();
        e.Data.Add("ApiError", error);
        throw e;
    }
}