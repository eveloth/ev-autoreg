using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class QueryParametersResponseExample : IExamplesProvider<Response<QueryParametersDto>>
{
    public Response<QueryParametersDto> GetExamples()
    {
        return new Response<QueryParametersDto>(
            new QueryParametersDto
            {
                IssueType = new IssueTypeDto
                {
                    Id = 3,
                    IssueTypeName = "Networking / Legal Entity"
                },
                WorkTime = "worktime=4",
                RegStatus = "status=registered",
                InWorkStatus = null,
                AssignedGroup = "assigned_group=techsupport",
                RequestType = "reqtype=notification",
            }
        );
    }
}