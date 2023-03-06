using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class NewRuleRequestExample : IExamplesProvider<RuleRequest>
{
    public RuleRequest GetExamples()
    {
        return new RuleRequest("server is not respondnig", 5, false, false);
    }
}