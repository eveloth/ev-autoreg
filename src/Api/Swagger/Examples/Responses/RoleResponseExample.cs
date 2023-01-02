using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class RoleResponseExample : IExamplesProvider<Response<RoleDto>>
{
    public Response<RoleDto> GetExamples()
    {
        return new Response<RoleDto>(new RoleDto { Id = 1, RoleName = "administrator" });
    }
}