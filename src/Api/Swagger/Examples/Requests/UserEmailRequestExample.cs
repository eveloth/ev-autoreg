using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class UserEmailRequestExample : IExamplesProvider<UserEmailRequest>
{
    public UserEmailRequest GetExamples()
    {
        return new UserEmailRequest("mynewemail@mail.com");
    }
}