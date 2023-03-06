using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class RolePermissionResponseExample : IExamplesProvider<Response<RolePermissionDto>>
{
    public Response<RolePermissionDto> GetExamples()
    {
        return new Response<RolePermissionDto>(
            new RolePermissionDto
            {
                RoleId = 1,
                RoleName = "administrator",
                IsPrivelegedRole = false,
                Permissions = new List<PermissionDto>
                {
                    new()
                    {
                        Id = 3,
                        PermissionName = "CreateRoles",
                        Description = "Can create new roles"
                    },
                    new()
                    {
                        Id = 1,
                        PermissionName = "ReadUsers",
                        Description = "Can view all users"
                    }
                }
            }
        );
    }
}