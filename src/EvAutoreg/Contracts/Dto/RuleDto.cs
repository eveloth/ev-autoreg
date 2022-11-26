namespace EvAutoreg.Contracts.Dto;

public class RuleDto
{
    public int Id { get; set; }
    public string Rule { get; set; }
    public IssueTypeDto IssueType { get; set; }
    public IssueFieldDto IssueField { get; set; }
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}