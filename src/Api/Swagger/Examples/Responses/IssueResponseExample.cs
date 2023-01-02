using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses;

public class IssueResponseExample : IExamplesProvider<Response<IssueDto>>
{
    public Response<IssueDto> GetExamples()
    {
        return new Response<IssueDto>(
            new IssueDto
            {
                Id = 100200,
                TimeCreated = DateTime.UtcNow,
                Author = "Karl Marx",
                Company = "Lucasfilm Ltd.",
                Status = "In work",
                Priority = "Critical",
                AssignedGroup = "Tech support",
                Assignee = "José Alberto Mujica Cordano",
                ShortDescription = "Cannot open monitoring dashboard",
                Description =
                    "We cannot access our monitoring dashboard due to some SSL certificate problem,\n"
                    + "please tell us what does SSL mean and send help.\n\n"
                    + "Regards, Karl Marx, Principal System Architect",
                RegistrarId = 17,
                RegistrarFirstName = "Ieyasu",
                RegistrarLastName = "Tokugawa",
                IssueType = new IssueTypeDto { Id = 3, IssueTypeName = "Networking / Legal Entity" }
            }
        );
    }
}