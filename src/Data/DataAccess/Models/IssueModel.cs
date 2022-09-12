namespace Data.DataAccess.Models;

public class IssueModel
{
    public int p_id { get; set; }
    public string p_issue_no { get; set; }
    public DateTime p_date_created { get; set; }
    public string p_author { get; set; } = string.Empty;
    public string p_company { get; set; } = string.Empty;
    public string p_status { get; set; }
    public string p_priority { get; set; }
    public string p_assigned_group { get; set; } = string.Empty;
    public string p_assignee { get; set; } = string.Empty;
    public string p_description { get; set; }
}