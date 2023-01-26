using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class UserEmailRequestExample : IExamplesProvider<UserEmailRequest>
{
    public UserEmailRequest GetExamples()
    {
        return new UserEmailRequest("mynewemail@mail.com");
    }
}