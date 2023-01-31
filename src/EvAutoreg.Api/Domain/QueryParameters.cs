namespace EvAutoreg.Api.Domain;

public class QueryParameters
{
#pragma warning disable CS8618
    public int Id { get; set; }
    public IssueType IssueType { get; set; }
    public string WorkTime { get; set; }
    public string Status { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
    public int ExecutionOrder { get; set; }
#pragma warning restore CS8618
}