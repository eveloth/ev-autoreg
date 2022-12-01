namespace Api.Contracts.Dto;

public class MailAnalysisRulesDto
{
    public required string NewIssueRegex { get; set; }
    public required string IssueNoRegex { get; set; }
}