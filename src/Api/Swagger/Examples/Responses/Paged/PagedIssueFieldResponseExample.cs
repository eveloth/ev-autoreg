using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedIssueFieldResponseExample : IExamplesProvider<PagedResponse<IssueFieldDto>>
{
    private IssueFieldDto[] _issueFields =
    {
        new() { Id = 1, FieldName = "Author" },
        new() { Id = 9, FieldName = "Description" },
    };

    public PagedResponse<IssueFieldDto> GetExamples()
    {
        return new PagedResponse<IssueFieldDto>(_issueFields, Pagination.Example);
    }
}