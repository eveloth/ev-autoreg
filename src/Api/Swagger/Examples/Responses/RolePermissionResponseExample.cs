using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Domain;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class RolePermissionResponseExample : IExamplesProvider<Response<RolePermissionDto>>
{
    public Response<RolePermissionDto> GetExamples()
    {
        return new Response<RolePermissionDto>(
            new RolePermissionDto
            {
                Role = new RoleDto { Id = 1, RoleName = "administrator" },
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