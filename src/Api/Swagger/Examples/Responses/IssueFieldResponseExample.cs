using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class IssueFieldResponseExample : IExamplesProvider<Response<IssueFieldDto>>
{
    public Response<IssueFieldDto> GetExamples()
    {
        return new Response<IssueFieldDto>(new IssueFieldDto
        {
            Id = 1,
            FieldName = "Author"
        });
    }
}