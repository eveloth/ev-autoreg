namespace EvAutoreg.Api.Contracts.Dto;

public class RuleDto
{
    public int Id { get; set; }
    public int RuleSetId { get; set; }
    public string RuleSubstring { get; set; } = default!;
    public IssueFieldDto IssueField { get; set; } = default!;
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}