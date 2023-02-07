namespace EvAutoreg.Api.Domain;

public class QueryParameters
{
    public int Id { get; set; }
    public IssueType IssueType { get; set; } = default!;
    public string WorkTime { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
    public int ExecutionOrder { get; set; }
}