using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class IssueTypeRequestExample : IExamplesProvider<IssueTypeRequest>
{
    public IssueTypeRequest GetExamples()
    {
        return new IssueTypeRequest("Bug report");
    }
}