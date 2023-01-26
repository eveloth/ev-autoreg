using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class UserResponseExample : IExamplesProvider<Response<UserDto>>
{
    public Response<UserDto> GetExamples()
    {
        return new Response<UserDto>(
            new UserDto
            {
                Id = 45,
                Email = "ryan@evautoreg.org",
                FirstName = "Ryan",
                LastName = "Gosling",
                IsBlocked = false,
                IsDeleted = false,
                Role = new RoleDto { Id = 6, RoleName = "supervisor" }
            }
        );
    }
}