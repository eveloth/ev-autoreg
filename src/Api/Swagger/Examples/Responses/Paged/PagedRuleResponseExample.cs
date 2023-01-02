using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedRuleResponseExample : IExamplesProvider<PagedResponse<RuleDto>>
{
    private RuleDto[] _rules =
    {
        new()
        {
            Id = 6,
            Rule = "Server is not responding",
            IssueType = new IssueTypeDto { Id = 3, IssueTypeName = "Networking / Legal Entity" },
            IssueField = new IssueFieldDto { Id = 9, FieldName = "Description" },
            IsRegex = false,
            IsNegative = false
        },
        new()
        {
            Id = 6,
            Rule = "Offer",
            IssueType = new IssueTypeDto { Id = 7, IssueTypeName = "Offers" },
            IssueField = new IssueFieldDto { Id = 9, FieldName = "Description" },
            IsRegex = false,
            IsNegative = false
        }
    };

    public PagedResponse<RuleDto> GetExamples()
    {
        return new PagedResponse<RuleDto>(_rules, Pagination.Example);
    }
}