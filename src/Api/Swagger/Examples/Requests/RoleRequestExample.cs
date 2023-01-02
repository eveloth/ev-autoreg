using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class RoleRequestExample : IExamplesProvider<RoleRequest>
{
    public RoleRequest GetExamples()
    {
        return new RoleRequest("manager");
    }
}