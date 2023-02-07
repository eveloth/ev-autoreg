using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Errors;
using EvAutoreg.Api.Exceptions;

namespace EvAutoreg.Api.Extensions;

public static class ApiExceptionExtensions
{
    public static ErrorResponse? ToErrorResponse(this ApiException e)
    {
        return e.Data["ApiError"] is not ApiError apiError
            ? null
            : new ErrorResponse { ApiError = apiError };
    }
}