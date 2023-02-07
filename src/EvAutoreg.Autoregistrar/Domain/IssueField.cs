namespace EvAutoreg.Autoregistrar.Domain;

public class IssueField
{
    public required int Id { get; set; }
    public required string FieldName { get; set; }
    public required List<Rule> Rules { get; set; }
}