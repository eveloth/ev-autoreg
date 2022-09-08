using Data.DataAccess.Models;

namespace Data.DataAccess;

public static class IssueData
{
    public static void PrintIssue(IssueModel issue)
    {
        Console.WriteLine($"{issue.DateCreated}\n{issue.IssueNo}\n{issue.Author}\n{issue.Company}\n{issue.Status}\n{issue.Priority}\n{issue.Description}");
    }
}
