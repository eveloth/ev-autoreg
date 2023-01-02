using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class RuleRequestExample : IExamplesProvider<RuleRequest>
{
    public RuleRequest GetExamples()
    {
        return new RuleRequest("server is not responding", 5, 9, false, false);
    }
}