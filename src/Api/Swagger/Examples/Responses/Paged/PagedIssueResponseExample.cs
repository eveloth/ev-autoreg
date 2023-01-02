using Api.Contracts.Dto;
using Api.Contracts.Responses;
using Api.Swagger.Examples.Templates;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Swagger.Examples.Responses.Paged;

public class PagedIssueResponseExample : IExamplesProvider<PagedResponse<IssueDto>>
{
    private IssueDto[] _issues =
    {
        new()
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
        },
        new()
        {
            Id = 100300,
            TimeCreated = DateTime.UtcNow,
            Author = "Andrea Tyson",
            Company = "JetBrains",
            Status = "Registered",
            Priority = "Normal",
            AssignedGroup = "Sales",
            Assignee = "Maria DeNiro",
            ShortDescription = "Infrastructure scaling assistance",
            Description =
                "We'd like to schedule a meeting to discuss your sale offer! Kind regards, Andrea",
            RegistrarId = 19,
            RegistrarFirstName = "John",
            RegistrarLastName = "Cena",
            IssueType = new IssueTypeDto { Id = 7, IssueTypeName = "Offers" }
        }
    };

    public PagedResponse<IssueDto> GetExamples()
    {
        return new PagedResponse<IssueDto>(_issues, Pagination.Example);
    }
}