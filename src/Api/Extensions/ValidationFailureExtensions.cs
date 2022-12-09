using Api.Contracts.Responses;
using FluentValidation.Results;
using static Api.Errors.ErrorCodes;

namespace Api.Extensions;

public static class ValidationFailureExtensions
{
    public static ErrorResponse ToErrorResponse(this ValidationResult failure)
    {
        var errorList = failure.Errors.Select(x => x.ErrorMessage).ToList();

        var response = new ErrorResponse
        {
            ApiError = ErrorCode[11001],
            Details = errorList
        };

        return response;
    }
}