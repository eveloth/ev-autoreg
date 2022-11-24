namespace DataAccessLibrary.Models;

public class MailAnalysisRuleModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string NewIssueRegex { get; set; }
    public string IssueNoRegex { get; set; }
    
#pragma warning restore
}