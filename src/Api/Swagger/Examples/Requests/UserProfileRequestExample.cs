using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class UserProfileRequestExample : IExamplesProvider<UserProfileRequest>
{
    public UserProfileRequest GetExamples()
    {
        return new UserProfileRequest("Jack", "Sparrow");
    }
}