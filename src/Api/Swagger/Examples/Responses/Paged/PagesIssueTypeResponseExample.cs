using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagesIssueTypeResponseExample : IExamplesProvider<PagedResponse<IssueTypeDto>>
{
    private IssueTypeDto[] _issueTypes =
    {
        new() { Id = 7, IssueTypeName = "Offers" },
        new() { Id = 3, IssueTypeName = "Networking / Legal Entity" }
    };

    public PagedResponse<IssueTypeDto> GetExamples()
    {
        return new PagedResponse<IssueTypeDto>(_issueTypes, PaginationTemplate.Example);
    }
}