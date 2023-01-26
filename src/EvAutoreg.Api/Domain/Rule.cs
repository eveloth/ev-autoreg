namespace EvAutoreg.Api.Domain;

public class Rule
{
#pragma warning disable CS8618
    public int Id { get; set; }
    public string RuleSubstring { get; set; }
    public int OwnerUserId { get; set; }
    public IssueType IssueType { get; set; }
    public IssueField IssueField { get; set; }
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
#pragma warning restore CS8618
}