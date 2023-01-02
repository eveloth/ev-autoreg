using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class AutoregistrarSettingsResponseExample
    : IExamplesProvider<Response<AutoregistrarSettingsDto>>
{
    public Response<AutoregistrarSettingsDto> GetExamples()
    {
        return new Response<AutoregistrarSettingsDto>(
            new AutoregistrarSettingsDto
            {
                ExchangeServerUri = "mail.domain.com",
                ExtraViewUri = "ev.domain.com",
                NewIssueRegex = "^\\[New Issue \\d{6}\\]",
                IssueNoRegex = "^\\[New Issue (\\d{6})\\]"
            }
        );
    }
}