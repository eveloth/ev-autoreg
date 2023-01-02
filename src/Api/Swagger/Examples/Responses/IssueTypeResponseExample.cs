using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class IssueTypeResponseExample : IExamplesProvider<Response<IssueTypeDto>>
{
    public Response<IssueTypeDto> GetExamples()
    {
        return new Response<IssueTypeDto>(
            new IssueTypeDto { Id = 3, IssueTypeName = "Networking / Legal Entity" }
        );
    }
}