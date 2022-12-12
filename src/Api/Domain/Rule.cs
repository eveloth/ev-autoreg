namespace Api.Domain;

public class Rule
{
    public int Id { get; set; }
    public string RuleSubstring { get; set; }
    public int OwnerUserId { get; set; }
    public IssueType IssueType { get; set; }
    public IssueField IssueField { get; set; }
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}