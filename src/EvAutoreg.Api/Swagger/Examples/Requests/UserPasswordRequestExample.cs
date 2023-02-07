using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class UserPasswordRequestExample : IExamplesProvider<UserPasswordRequest>
{
    public UserPasswordRequest GetExamples()
    {
        return new UserPasswordRequest("Mys3cretnewp@ssword");
    }
}