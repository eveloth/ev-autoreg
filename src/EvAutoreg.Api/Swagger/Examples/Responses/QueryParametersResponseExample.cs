using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class QueryParametersResponseExample
    : IExamplesProvider<Response<IEnumerable<QueryParametersDto>>>
{
    public Response<IEnumerable<QueryParametersDto>> GetExamples()
    {
        return new Response<IEnumerable<QueryParametersDto>>(
            new[]
            {
                new QueryParametersDto
                {
                    IssueType = new IssueTypeDto
                    {
                        Id = 3,
                        IssueTypeName = "Networking / Legal Entity"
                    },
                    WorkTime = "worktime=4",
                    Status = "status=registered",
                    AssignedGroup = "assigned_group=techsupport",
                    RequestType = "reqtype=notification",
                }
            }
        );
    }
}