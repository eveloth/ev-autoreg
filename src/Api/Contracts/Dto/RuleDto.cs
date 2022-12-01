namespace Api.Contracts.Dto;

public class RuleDto
{
    public required int Id { get; set; }
    public required string Rule { get; set; }
    public required IssueTypeDto? IssueType { get; set; }
    public required IssueFieldDto? IssueField { get; set; }
    public required bool IsRegex { get; set; }
    public required bool IsNegative { get; set; }
}