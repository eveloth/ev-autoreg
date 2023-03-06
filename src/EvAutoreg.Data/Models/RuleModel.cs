namespace EvAutoreg.Data.Models;

public class RuleModel
{
    public int Id { get; set; }
    public int RuleSetId { get; set; }
    public string Rule { get; set; } = default!;
    public int IssueFieldId { get; set; }
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}