namespace EvAutoreg.Data.Models;

public class FilledRuleModel
{
    public int Id { get; set; }
    public int RuleSetId { get; set; }
    public string Rule { get; set; } = default!;
    public IssueFieldModel IssueField { get; set; } = default!;
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}