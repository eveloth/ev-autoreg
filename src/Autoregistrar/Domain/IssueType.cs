namespace Autoregistrar.Domain;

public class IssueType
{
    public required int Id { get; set; }
    public required string IssueTypeName { get; set; }
    public required QueryParameters QueryParameters { get; set; }
}