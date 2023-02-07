namespace EvAutoreg.Data.Models;

public class QueryParametersModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public int IssueTypeId { get; set; }
    public string WorkTime { get; set; }
    public string Status { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
    public int ExecutionOrder { get; set; }

#pragma warning restore
}