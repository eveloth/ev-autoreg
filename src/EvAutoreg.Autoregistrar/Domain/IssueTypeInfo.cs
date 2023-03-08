namespace EvAutoreg.Autoregistrar.Domain;

public class IssueTypeInfo
{
    public required int Id { get; set; }
    public required string IssueTypeName { get; set; }
    public required IEnumerable<QueryParameters> QueryParameters { get; set; }
    public required IEnumerable<RuleSet> RuleSets { get; set; }
}