using Api.Contracts.Responses;
using Api.Errors;
using Api.Exceptions;

namespace Api.Extensions;

public static class ApiExceptionExtensions
{
    public static ErrorResponse? ToErrorResponse(this ApiException e)
    {
        return e.Data["ApiError"] is not ApiError apiError
            ? null
            : new ErrorResponse { ApiError = apiError };
    }
}