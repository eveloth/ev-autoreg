using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class QueryParametersRequestExample : IExamplesProvider<QueryParametersRequest>
{
    public QueryParametersRequest GetExamples()
    {
        return new QueryParametersRequest(
            "worktime=4",
            "status=registered",
            null,
            "assigned_group=techsupport",
            "reqtype=notification"
        );
    }
}