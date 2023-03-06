namespace EvAutoreg.Autoregistrar.Domain;

public class RuleSet
{
    public int Id { get; set; }
    public int OwnerUserId { get; set; }
    public IssueType IssueType { get; set; } = default!;
    public List<Rule> Rules { get; set; } = new();
}