using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses.Paged;

public class PagedRolePermissionResponseExample
    : IExamplesProvider<PagedResponse<RolePermissionDto>>
{
    private RolePermissionDto[] _rolePermissions =
    {
        new RolePermissionDto
        {
            RoleId = 1,
            RoleName = "administrator",
            IsPrivelegedRole = false,
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
            RoleId = 3,
            RoleName = "manager",
            IsPrivelegedRole = false,
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
        return new PagedResponse<RolePermissionDto>(
            _rolePermissions,
            PaginationTemplate.Example,
            2
        );
    }
}