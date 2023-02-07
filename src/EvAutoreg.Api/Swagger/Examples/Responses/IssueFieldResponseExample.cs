using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

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