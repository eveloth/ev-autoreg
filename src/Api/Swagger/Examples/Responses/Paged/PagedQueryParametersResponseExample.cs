using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedQueryParametersResponseExample
    : IExamplesProvider<PagedResponse<QueryParametersDto>>
{
    private QueryParametersDto[] _queryParameters =
    {
        new()
        {
            IssueType = new IssueTypeDto { Id = 3, IssueTypeName = "Networking / Legal Entity" },
            WorkTime = "worktime=4",
            RegStatus = "status=registered",
            InWorkStatus = null,
            AssignedGroup = "assigned_group=techsupport",
            RequestType = "reqtype=notification",
        },
        new()
        {
            IssueType = new IssueTypeDto { Id = 7, IssueTypeName = "Offers" },
            WorkTime = "worktime=4",
            RegStatus = "status=registered",
            InWorkStatus = null,
            AssignedGroup = "assigned_group=sales",
            RequestType = "reqtype=notification",
        }
    };

    public PagedResponse<QueryParametersDto> GetExamples()
    {
        return new PagedResponse<QueryParametersDto>(_queryParameters, PaginationTemplate.Example);
    }
}