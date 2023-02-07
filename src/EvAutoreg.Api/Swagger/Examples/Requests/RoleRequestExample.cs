using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class RoleRequestExample : IExamplesProvider<RoleRequest>
{
    public RoleRequest GetExamples()
    {
        return new RoleRequest("manager");
    }
}