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
        var results = await _db.LoadData<IssueModel, int>("p_get_issue", id);
        return results.FirstOrDefault();
    }

    public async Task UpsertIssue(dynamic issue)
    {
        var parameters = new DynamicParameters(issue);
        await _db.SaveData("p_upsert_issue", parameters);
    }
}
