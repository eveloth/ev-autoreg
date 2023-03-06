using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class RuleSetRequestExample : IExamplesProvider<RuleSetRequest>
{
    public RuleSetRequest GetExamples()
    {
        return new RuleSetRequest(1);
    }
}