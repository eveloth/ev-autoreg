namespace DataAccessLibrary.Models;

public class RuleModel
{
    public int Id { get; set; }
    public string Rule { get; set; }
    public int OwnerUserId { get; set; }
    public int IssueTypeid { get; set; }
    public int AnalysisAreaId { get; set; }
    public bool IsRegex { get; set; }
    public bool IsNegative { get; set; }
}