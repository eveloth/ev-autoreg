using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses.Paged;

public class PagedIssueFieldResponseExample : IExamplesProvider<PagedResponse<IssueFieldDto>>
{
    private IssueFieldDto[] _issueFields =
    {
        new() { Id = 1, FieldName = "Author" },
        new() { Id = 9, FieldName = "Description" },
    };

    public PagedResponse<IssueFieldDto> GetExamples()
    {
        return new PagedResponse<IssueFieldDto>(_issueFields, PaginationTemplate.Example, 2);
    }
}