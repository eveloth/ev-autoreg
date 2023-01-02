using Api.Contracts.Responses;
using Api.Errors;
using Swashbuckle.AspNetCore.Filters;
using static Api.Errors.ErrorCodes;

namespace Api.Swagger.Examples.Responses;

public class ErrorResponseExample : IExamplesProvider<ErrorResponse>
{
    public ErrorResponse GetExamples()
    {
        return new ErrorResponse
        {
            ApiError = ErrorCode[13001],
            Details = new List<string> { "Error details if there are any." }
        };
    }
}