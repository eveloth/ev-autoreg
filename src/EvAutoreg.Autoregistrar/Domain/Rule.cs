namespace EvAutoreg.Autoregistrar.Domain;

public class Rule
{
    public int Id { get; set; }
    public int RuleSetId { get; set; }
    public string RuleSubstring { get; set; } = default!;
    public IssueField IssueField { get; set; } = default!;
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}