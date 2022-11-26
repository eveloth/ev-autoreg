namespace EvAutoreg.Contracts.Dto;

public class MailAnalysisRulesDto
{
    public int Id { get; set; }
    public string NewIssueRegex { get; set; }
    public string IssueNoRegex { get; set; }
}