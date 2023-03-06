using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class RuleSetResponseExample : IExamplesProvider<Response<RuleSetDto>>
{
    public Response<RuleSetDto> GetExamples()
    {
        return new Response<RuleSetDto>(
            new RuleSetDto
            {
                Id = 1,
                IssueType = new IssueTypeDto
                {
                    Id = 3,
                    IssueTypeName = "Networking / Legal Entity"
                },
                Rules = new List<RuleDto>
                {
                    new()
                    {
                        Id = 4,
                        RuleSetId = 1,
                        RuleSubstring = "Lucasfilm Ltd.",
                        IssueField = new IssueFieldDto { Id = 2, FieldName = "Company" },
                        IsRegex = false,
                        IsNegative = false
                    },
                    new()
                    {
                        Id = 5,
                        RuleSetId = 1,
                        RuleSubstring = "server is not responding",
                        IssueField = new IssueFieldDto { Id = 5, FieldName = "Description" },
                        IsRegex = false,
                        IsNegative = false
                    }
                }
            }
        );
    }
}