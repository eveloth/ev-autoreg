using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using EvAutoreg.Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses.Paged;

public class PagesIssueTypeResponseExample : IExamplesProvider<PagedResponse<IssueTypeDto>>
{
    private IssueTypeDto[] _issueTypes =
    {
        new() { Id = 7, IssueTypeName = "Offers" },
        new() { Id = 3, IssueTypeName = "Networking / Legal Entity" }
    };

    public PagedResponse<IssueTypeDto> GetExamples()
    {
        return new PagedResponse<IssueTypeDto>(_issueTypes, PaginationTemplate.Example, 2);
    }
}