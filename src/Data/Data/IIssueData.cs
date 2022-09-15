using Data.Models;

namespace Data.Data;

public interface IIssueData
{
    void PrintIssue(XmlIssueModel xmlIssue);
    Task<IEnumerable<IssueModel>> GetAllIssues();
    Task<IssueModel?> GetIssue(int id);
    Task UpsertIssue(dynamic issue);
}