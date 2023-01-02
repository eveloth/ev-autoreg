using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedPermissionResponseExample : IExamplesProvider<PagedResponse<PermissionDto>>
{
    private PermissionDto[] _permissions =
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
    };

    public PagedResponse<PermissionDto> GetExamples()
    {
        return new PagedResponse<PermissionDto>(_permissions, Pagination.Example);
    }
}