using EvAutoreg.Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Requests;

public class CredentialsRequestExample : IExamplesProvider<UserCredentialsRequest>
{
    public UserCredentialsRequest GetExamples()
    {
        return new UserCredentialsRequest("user@evautoreg.org", "P@ssw0rd123");
    }
}