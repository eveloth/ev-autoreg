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

    public TestDbWriter(Rules rules, IEVApiWrapper evapi, IIssueData issueData, IConfiguration config)
    {
        _rules = rules;
        _evapi = evapi;
        _issueData = issueData;
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

            await _issueData.UpsertIssue(issue);
        }
    }
}
