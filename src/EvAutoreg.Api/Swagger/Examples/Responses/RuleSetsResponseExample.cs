using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class RuleSetsResponseExample : IExamplesProvider<Response<IEnumerable<RuleSetDto>>>
{
    public Response<IEnumerable<RuleSetDto>> GetExamples()
    {
        return new Response<IEnumerable<RuleSetDto>>(
            new[]
            {
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
                            Id = 3,
                            RuleSetId = 1,
                            RuleSubstring = "Amanda Cassio",
                            IssueField = new IssueFieldDto { Id = 1, FieldName = "Author" },
                            IsRegex = false,
                            IsNegative = false
                        },
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
                },
                new RuleSetDto
                {
                    Id = 2,
                    IssueType = new IssueTypeDto
                    {
                        Id = 3,
                        IssueTypeName = "Networking / Legal Entity"
                    },
                    Rules = new List<RuleDto>
                    {
                        new()
                        {
                            Id = 6,
                            RuleSetId = 1,
                            RuleSubstring =
                                @"\[CORP\]\s\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\sUNAVAILABLE",
                            IssueField = new IssueFieldDto
                            {
                                Id = 4,
                                FieldName = "ShortDescription"
                            },
                            IsRegex = true,
                            IsNegative = false
                        },
                    }
                }
            }
        );
    }
}