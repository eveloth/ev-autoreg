namespace EvAutoreg.Api.Domain;

public class Rule
{
    public int Id { get; set; }
    public string RuleSubstring { get; set; } = default!;
    public int OwnerUserId { get; set; }
    public IssueType IssueType { get; set; } = default!;
    public IssueField IssueField { get; set; } = default!;
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}