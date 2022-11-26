namespace EvAutoreg.Contracts.Dto;

public class IssueDto
{
    public int Id { get; set; }
    public DateTime TimeCreated { get; set; }
    public string Author { get; set; }
    public string Company { get; set; }
    public string Priority { get; set; }
    public string? AssignedGroup { get; set; }
    public string? Assignee { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public UserProfileDto Registrar { get; set; }
    public IssueTypeDto IssueType { get; set; }
}
