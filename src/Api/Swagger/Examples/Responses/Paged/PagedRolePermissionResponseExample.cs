using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedRolePermissionResponseExample
    : IExamplesProvider<PagedResponse<RolePermissionDto>>
{
    private RolePermissionDto[] _rolePermissions =
    {
        new RolePermissionDto
        {
            Role = new RoleDto { Id = 1, RoleName = "administrator" },
            Permissions = new List<PermissionDto>
            {
                new()
                {
                    Id = 1,
                    PermissionName = "ReadUsers",
                    Description = "Can view all users"
                },
                new()
                {
                    Id = 2,
                    PermissionName = "UpdateUsers",
                    Description = "Can update users"
                },
                new()
                {
                    Id = 3,
                    PermissionName = "CreateRoles",
                    Description = "Can create new roles"
                },
            }
        },
        new RolePermissionDto
        {
            Role = new RoleDto { Id = 3, RoleName = "manager" },
            Permissions = new List<PermissionDto>
            {
                new()
                {
                    Id = 1,
                    PermissionName = "ReadUsers",
                    Description = "Can view all users"
                },
            }
        }
    };

    public PagedResponse<RolePermissionDto> GetExamples()
    {
        return new PagedResponse<RolePermissionDto>(_rolePermissions, PaginationTemplate.Example);
    }
}