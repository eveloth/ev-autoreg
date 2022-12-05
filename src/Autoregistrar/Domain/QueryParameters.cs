namespace Autoregistrar.Domain;

public class QueryParameters
{
    public required string WorkTime { get; set; }
    public required string RegStatus { get; set; }
    public string? InWorkStatus { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
}