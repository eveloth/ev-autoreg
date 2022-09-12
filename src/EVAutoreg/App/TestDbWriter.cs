using System.Text.RegularExpressions;
using Data.Data;
using Data.Extensions;
using EVAutoreg.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Task = System.Threading.Tasks.Task;

namespace EVAutoreg.App;

public class TestDbWriter : IMailEventListener
{
    private readonly IEVApiWrapper _evapi;
    private readonly Rules _rules;
    private readonly IIssueData _issueData;
    private readonly IConfiguration _config;

    public TestDbWriter(Rules rules, IEVApiWrapper evapi, IIssueData issueData, IConfiguration config)
    {
        _rules = rules;
        _evapi = evapi;
        _issueData = issueData;
        _config = config;
    }

    public async Task ProcessEvent(EmailMessage email)
    {
        var subject = email.Subject;
        var issueNo = Regex.Match(subject, _rules.RegexIssueNo).Groups[1].Value;

        var xmlIssue = await _evapi.GetIssue(issueNo);

        if (xmlIssue is not null)
        {
            _issueData.PrintIssue(xmlIssue);
            var issue = xmlIssue.ConvertToSqlModel();
            //                var sql =
            //                    $@"INSERT INTO issue (issue_no, date_created, author, company, status, priority, assigned_group, assignee, description)
            //VALUES (@issue_no, @date_created_utc, @author, @company, @status, @priority, @assigned_group, @assignee, @description)";

            var parameters = new
            {
                issue.p_issue_no,
                issue.p_date_created,
                issue.p_author,
                issue.p_company,
                issue.p_status,
                issue.p_priority,
                issue.p_assigned_group,
                issue.p_assignee,
                issue.p_description
            };

            await _issueData.UpsertIssue(parameters);
        }
    }
}
