namespace EvAutoreg.Contracts.Dto;

public class MailAnalysisRulesDto
{
    public required int Id { get; set; }
    public required string NewIssueRegex { get; set; }
    public required string IssueNoRegex { get; set; }
}