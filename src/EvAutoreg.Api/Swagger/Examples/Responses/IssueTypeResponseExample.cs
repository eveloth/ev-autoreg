using EvAutoreg.Api.Contracts.Dto;
using EvAutoreg.Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace EvAutoreg.Api.Swagger.Examples.Responses;

public class IssueTypeResponseExample : IExamplesProvider<Response<IssueTypeDto>>
{
    public Response<IssueTypeDto> GetExamples()
    {
        return new Response<IssueTypeDto>(
            new IssueTypeDto { Id = 3, IssueTypeName = "Networking / Legal Entity" }
        );
    }
}