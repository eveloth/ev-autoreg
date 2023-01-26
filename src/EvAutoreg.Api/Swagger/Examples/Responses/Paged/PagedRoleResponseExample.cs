using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses.Paged;

public class PagedRoleResponseExample : IExamplesProvider<PagedResponse<RoleDto>>
{
    private RoleDto[] _roles =
    {
        new() { Id = 1, RoleName = "administrator" },
        new() { Id = 3, RoleName = "supervisor" }
    };

    public PagedResponse<RoleDto> GetExamples()
    {
        return new PagedResponse<RoleDto>(_roles, PaginationTemplate.Example);
    }
}