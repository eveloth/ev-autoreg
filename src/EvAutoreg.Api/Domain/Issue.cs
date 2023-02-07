namespace EvAutoreg.Api.Domain;

public class Issue
{
    public int Id { get; set; }
    public DateTime TimeCreated { get; set; }
    public string Author { get; set; } = default!;
    public string Company { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public string? AssignedGroup { get; set; }
    public string? Assignee { get; set; }
    public string ShortDescription { get; set; } = default!;
    public string Description { get; set; } = default!;
    public User? Registrar { get; set; }
    public IssueType? IssueType { get; set; }
}