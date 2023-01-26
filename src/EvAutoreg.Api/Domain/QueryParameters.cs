namespace EvAutoreg.Api.Domain;

public class QueryParameters
{
#pragma warning disable CS8618
    public IssueType IssueType { get; set; }
    public string WorkTime { get; set; }
    public string RegStatus { get; set; }
    public string? InWorkStatus { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
#pragma warning restore CS8618
}