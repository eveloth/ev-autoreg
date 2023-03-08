namespace EvAutoreg.Data.Models;

public class FilledRuleSetModel
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; }
    public IssueTypeModel IssueType { get; set; } = default!;
    public List<FilledRuleModel> Rules { get; set; } = new();
}