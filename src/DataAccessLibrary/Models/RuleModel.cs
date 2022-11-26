namespace DataAccessLibrary.Models;

public class RuleModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string Rule { get; set; }
    public int OwnerUserId { get; set; }
    public int IssueTypeId { get; set; }
    public int IssueFieldId { get; set; }
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }

#pragma warning restore
}
