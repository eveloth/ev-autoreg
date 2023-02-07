namespace EvAutoreg.Api.Contracts.Dto;

public class QueryParametersDto
{
    public int Id { get; set; }
    public required IssueTypeDto IssueType { get; set; }
    public required string WorkTime { get; set; }
    public required string Status { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
    public int ExecutionOrder { get; set; }
}