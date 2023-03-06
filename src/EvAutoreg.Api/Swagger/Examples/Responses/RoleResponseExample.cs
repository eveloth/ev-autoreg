using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class RoleResponseExample : IExamplesProvider<Response<RoleDto>>
{
    public Response<RoleDto> GetExamples()
    {
        return new Response<RoleDto>(new RoleDto { Id = 1, RoleName = "administrator" });
    }
}