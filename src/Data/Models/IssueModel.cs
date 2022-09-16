namespace Data.Models;

public class IssueModel
{
    public int Id { get; set; }
    public string IssueNo { get; set; }
    public DateTime DateCreated { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string Status { get; set; }
    public string Priority { get; set; }
    public string AssignedGroup { get; set; } = string.Empty;
    public string Assignee { get; set; } = string.Empty;
    public string Description { get; set; }
}