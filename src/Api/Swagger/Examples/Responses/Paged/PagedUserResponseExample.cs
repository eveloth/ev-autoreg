using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedUserResponseExample : IExamplesProvider<PagedResponse<UserDto>>
{
    private UserDto[] _users =
    {
        new()
        {
            Id = 45,
            Email = "ryan@evautoreg.org",
            FirstName = "Ryan",
            LastName = "Gosling",
            IsBlocked = false,
            IsDeleted = false,
            Role = new RoleDto { Id = 6, RoleName = "supervisor" }
        },
        new()
        {
            Id = 63,
            Email = "jack@evautoreg.org",
            FirstName = "Jack",
            LastName = "Sparrow",
            IsBlocked = true,
            IsDeleted = false,
            Role = new RoleDto { Id = 3, RoleName = "manager" }
        },
    };

    public PagedResponse<UserDto> GetExamples()
    {
        return new PagedResponse<UserDto>(_users, PaginationTemplate.Example);
    }
}