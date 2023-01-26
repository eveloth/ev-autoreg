using EvAutoreg.Api.Contracts.Responses;
using FluentValidation;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Extensions;

public static class ValidationExceptiionExtensions
{
    public static ErrorResponse ToErrorResponse(this ValidationException e)
    {
        var errorList = e.Errors.Select(x => x.ErrorMessage).ToList();

        var response = new ErrorResponse
        {
            ApiError = ErrorCode[12001],
            Details = errorList
        };

        return response;
    }
}