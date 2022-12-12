using Api.Contracts.Responses;
using FluentValidation;
using FluentValidation.Results;
using static Api.Errors.ErrorCodes;

namespace Api.Extensions;

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