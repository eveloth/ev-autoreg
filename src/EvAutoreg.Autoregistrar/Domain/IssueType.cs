namespace EvAutoreg.Autoregistrar.Domain;

public class IssueType
{
    public required int Id { get; set; }
    public required string IssueTypeName { get; set; }
    public required IEnumerable<QueryParameters> QueryParameters { get; set; }
}