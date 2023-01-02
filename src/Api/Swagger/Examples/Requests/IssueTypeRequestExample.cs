using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class IssueTypeRequestExample : IExamplesProvider<IssueTypeRequest>
{
    public IssueTypeRequest GetExamples()
    {
        return new IssueTypeRequest("Bug report");
    }
}