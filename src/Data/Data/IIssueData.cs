﻿using Data.Models;

namespace Data.Data;

public interface IIssueData
{
    void PrintIssue(XmlIssueModel xmlIssue);
    Task<IEnumerable<IssueModel>> GetAllIssues();
    Task<IssueModel?> GetIssue(string issueNo);
    Task UpsertIssue(IssueModel issue);
}
