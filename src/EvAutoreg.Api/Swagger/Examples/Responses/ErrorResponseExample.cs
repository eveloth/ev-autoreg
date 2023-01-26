using EvAutoreg.Api.Errors;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

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