using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class ExternalCredentialsRequestExample : IExamplesProvider<ExternalCredentialsRequest>
{
    public ExternalCredentialsRequest GetExamples()
    {
        return new ExternalCredentialsRequest("user@domain.com", "P@ssw0rd123");
    }
}