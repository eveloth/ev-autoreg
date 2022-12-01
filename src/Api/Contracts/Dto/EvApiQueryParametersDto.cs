namespace Api.Contracts.Dto;

public class EvApiQueryParametersDto
{
    public required IssueTypeDto IssueType { get; set; }
    public required string WorkTime { get; set; }
    public required string RegStatus { get; set; }
    public string? InWorkStatus { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
}