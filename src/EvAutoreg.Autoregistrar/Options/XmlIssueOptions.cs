namespace EvAutoreg.Autoregistrar.Options;

public class XmlIssueOptions
{
    public string Root { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string TimeCreated { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Company { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public string AssignedGroup { get; set; } = default!;
    public string Assignee { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Description { get; set; } = default!;
}