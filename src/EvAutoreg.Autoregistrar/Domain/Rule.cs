namespace EvAutoreg.Autoregistrar.Domain;

public class Rule
{
    public required string RuleSubstring { get; set; }
    public required int IssueTypeId { get; set; }
    public required bool IsRegex { get; set; }
    public required bool IsNegative { get; set; }
}