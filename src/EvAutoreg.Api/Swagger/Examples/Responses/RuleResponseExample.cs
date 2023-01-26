using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class RuleResponseExample : IExamplesProvider<Response<RuleDto>>
{
    public Response<RuleDto> GetExamples()
    {
        return new Response<RuleDto>(
            new RuleDto
            {
                Id = 6,
                Rule = "Server is not responding",
                IssueType = new IssueTypeDto
                {
                    Id = 3,
                    IssueTypeName = "Networking / Legal Entity"
                },
                IssueField = new IssueFieldDto { Id = 9, FieldName = "Description" },
                IsRegex = false,
                IsNegative = false
            }
        );
    }
}