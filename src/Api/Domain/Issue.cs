namespace Api.Domain;

public class Issue
{
    public int Id { get; set; }
    public DateTime TimeCreated { get; set; }
    public string Author { get; set; }
    public string Company { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public string? AssignedGroup { get; set; }
    public string? Assignee { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public User? Registrar { get; set; }
    public IssueType? IssueType { get; set; }
}