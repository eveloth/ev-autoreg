using Dapper;
using Data.Extensions;
using Data.Models;
using Data.SqlDataAccess;

namespace Data.Data;

public class IssueData : IIssueData
{
    private readonly ISqlDataAccess _db;

    public IssueData(ISqlDataAccess db)
    {
        _db = db;
    }

    public void PrintIssue(XmlIssueModel xmlIssue)
    {
        Console.WriteLine(
            $"{xmlIssue.DateCreated}\n{xmlIssue.IssueNo}\n{xmlIssue.Author}\n{xmlIssue.Company}\n{xmlIssue.Status}\n{xmlIssue.Priority}\n{xmlIssue.Description}");

        var issue = xmlIssue.ConvertToSqlModel();

        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"New datetime: {issue.DateCreated}");
    }

    public async Task<IEnumerable<IssueModel>> GetAllIssues() => await _db.LoadAll<IssueModel>("SELECT * FROM issue");

    public async Task<IssueModel?> GetIssue(int id)
    {
        var results = await _db.LoadData<IssueModel, int>("SELECT * FROM issue WHERE id = @Id", id);
        return results.FirstOrDefault();
    }

    public async Task UpsertIssue(dynamic issue)
    {
        var parameters = new DynamicParameters(issue);
        const string sql = @"INSERT INTO issue (issue_no, date_created, author, company,
                  status, priority, assigned_group, assignee, description) 
                  VALUES (@IssueNo, @DateCreated, @Author, @Company, @Status,
                  @Priority, @AssignedGroup, @Assignee, @Description)
                  ON CONFLICT (issue_no) DO UPDATE SET
                  author = EXCLUDED.author, company = EXCLUDED.company,
                  status = EXCLUDED.status, priority = EXCLUDED.priority,
                  assigned_group = EXCLUDED.assigned_group, assignee = EXCLUDED.assignee";

        await _db.SaveData(sql, parameters);
    }
}
