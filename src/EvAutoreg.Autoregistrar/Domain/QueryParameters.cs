namespace EvAutoreg.Autoregistrar.Domain;

public class QueryParameters
{
    public required string WorkTime { get; set; }
    public required string Status { get; set; }
    public string? AssignedGroup { get; set; }
    public string? RequestType { get; set; }
    public int ExecutionOrder { get; set; }
}