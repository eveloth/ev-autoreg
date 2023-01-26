using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class SuccessResponseExample : IExamplesProvider<SuccessResponse>
{
    public SuccessResponse GetExamples()
    {
        return new SuccessResponse(true);
    }
}