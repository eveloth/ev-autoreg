using Api.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Requests;

public class AutoregistrarSettingsRequestExample : IExamplesProvider<AutoregistrarSettingsRequest>
{
    public AutoregistrarSettingsRequest GetExamples()
    {
        return new AutoregistrarSettingsRequest(
            "mail.domain.com",
            "ev.domain.com",
            "^\\[New Issue \\d{6}\\]",
            "^\\[New Issue (\\d{6})\\]"
        );
    }
}