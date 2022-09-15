using Data.Models;

namespace Data.Extensions;

public static class IssueExtensions
{
    public static IssueModel ConvertToSqlModel(this XmlIssueModel xmlIssue)
    {
        return new IssueModel()
        {
            IssueNo = xmlIssue.IssueNo,
            DateCreated = DateTime.Parse(xmlIssue.DateCreated),
            Author = xmlIssue.Author,
            Company = xmlIssue.Company,
            Status = xmlIssue.Status,
            Priority = xmlIssue.Priority,
            AssignedGroup = xmlIssue.AssignedGroup,
            Assignee = xmlIssue.Assignee,
            Description = xmlIssue.Description
        };
    }
}