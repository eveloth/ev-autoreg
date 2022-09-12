using Data.DataAccess.Models;

namespace Data.Extensions;

public static class IssueExtensions
{
    public static IssueModel ConvertToSqlModel(this XmlIssueModel xmlIssue)
    {
        return new IssueModel()
        {
            p_issue_no = xmlIssue.IssueNo,
            p_date_created = DateTime.Parse(xmlIssue.DateCreated),
            p_author = xmlIssue.Author,
            p_company = xmlIssue.Company,
            p_status = xmlIssue.Status,
            p_priority = xmlIssue.Priority,
            p_assigned_group = xmlIssue.AssignedGroup,
            p_assignee = xmlIssue.Assignee,
            p_description = xmlIssue.Description
        };
    }
}