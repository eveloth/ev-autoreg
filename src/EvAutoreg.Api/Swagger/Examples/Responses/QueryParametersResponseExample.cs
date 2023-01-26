using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

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