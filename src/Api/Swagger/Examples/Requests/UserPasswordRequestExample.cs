using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class UserPasswordRequestExample : IExamplesProvider<UserPasswordRequest>
{
    public UserPasswordRequest GetExamples()
    {
        return new UserPasswordRequest("Mys3cretnewp@ssword");
    }
}