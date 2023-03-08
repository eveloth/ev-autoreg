namespace EvAutoreg.Api.Contracts.Dto;

public class RuleSetDto
{
    public int Id { get; set; }
    public IssueTypeDto IssueType { get; set; } = default!;
    public List<RuleDto> Rules { get; set; } = new();
}