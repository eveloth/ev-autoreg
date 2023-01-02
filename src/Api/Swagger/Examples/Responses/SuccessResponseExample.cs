using Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class SuccessResponseExample : IExamplesProvider<SuccessResponse>
{
    public SuccessResponse GetExamples()
    {
        return new SuccessResponse(true);
    }
}