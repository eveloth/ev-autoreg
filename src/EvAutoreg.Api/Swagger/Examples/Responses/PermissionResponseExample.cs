using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class PermissionResponseExample : IExamplesProvider<Response<PermissionDto>>
{
    public Response<PermissionDto> GetExamples()
    {
        return new Response<PermissionDto>(
            new PermissionDto
            {
                Id = 3,
                PermissionName = "CreateRoles",
                Description = "Can create new roles"
            }
        );
    }
}