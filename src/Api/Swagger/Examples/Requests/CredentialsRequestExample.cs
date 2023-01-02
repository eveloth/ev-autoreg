using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class CredentialsRequestExample : IExamplesProvider<UserCredentialsRequest>
{
    public UserCredentialsRequest GetExamples()
    {
        return new UserCredentialsRequest("user@evautoreg.org", "P@ssw0rd123");
    }
}