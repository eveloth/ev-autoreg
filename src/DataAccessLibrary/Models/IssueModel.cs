namespace DataAccessLibrary.Models;

public class IssueModel
{
#pragma warning disable CS8618
    
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
    public int RegistrarId { get; set; }
    public int IssueTypeId { get; set; }
    
#pragma warning restore
}
