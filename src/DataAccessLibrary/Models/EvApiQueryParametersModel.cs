namespace DataAccessLibrary.Models;

public class EvApiQueryParametersModel
{
#pragma warning disable CS8618

    public int IssueTypeId { get; set; }
    public string WorkTime { get; set; }
    public string RegStatus { get; set; }
    public string InWorkStatus { get; set; }
    public string AssignedGroup { get; set; }
    public string RequestType { get; set; }

#pragma warning restore
}
