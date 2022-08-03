using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using static EVAutoreg.PrettyPrinter;
using Task = System.Threading.Tasks.Task;

namespace EVAutoreg;

public class MailEventListener : IMailEventListener
{
    private readonly IConfiguration _config;
    private readonly IEVApiWrapper _evapi;
    private readonly Rules _rules;

    private enum IssueType
    {
        Monitoring,
        ExternalIT,
        None
    }

    public MailEventListener(IConfiguration config, IEVApiWrapper evapi, Rules rules)
    {
        _config = config;
        _evapi = evapi;
        _rules = rules;
    }

    public async Task ProcessEvent(EmailMessage email)
    {
        var subject = email.Subject;
        var body = email.Body.Text;
        var issueNo = Regex.Match(subject, @"^\[.+(\d{6})\]").Groups[1].Value;
        var rules = new Rules(_config);

        var action = IdentifyIssueType(subject, body, rules) switch
        {
            IssueType.Monitoring => RegisterAsMonitoring(issueNo),
            IssueType.ExternalIT => RegisterAsExternalIT(issueNo),
            _                    => Task.CompletedTask
        };

        await action;
    }


    private IssueType IdentifyIssueType(string subject, string body, Rules rules)
    {
        var subjectRules = _config.GetSection("MailAnalysisRules:SubjectRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var bodyRules = _config.GetSection("MailAnalysisRules:BodyRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var subjectNegativeRules = _config.GetSection("MailAnalysisRules:SubjectNegativeRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var bodyNegativeRules = _config.GetSection("MailAnalysisRules:BodyNegativeRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var externalITSubjectRules = _config.GetSection("MailAnalysisRules:ExternalITSubjectRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var externalITBodyRules = _config.GetSection("MailAnalysisRules:ExternalITBodyRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();

        if (IsMonitoring())
        {
            return IssueType.Monitoring;
        }
        else if(IsExternalIT())
        {
            return IssueType.ExternalIT;
        }
        else
        {
            return IssueType.None;
        }

        bool IsMonitoring()
        {
            return rules.SubjectRules.Any(subject.Contains) && !rules.SubjectNegativeRules.Any(subject.Contains) ||
                   rules.BodyRules.Any(body.Contains) && !rules.BodyNegativeRules.Any(body.Contains) ||
                   Regex.IsMatch(subject, @"^\[.+\]: Новое.{1,4}\d{6}(?:.{0,2})?$");
        }

        bool IsExternalIT()
        {
            return rules.ExternalITSubjectRules.Any(subject.Contains) ||
                   rules.ExternalITBodyRules.Any(body.Contains) ||
                   Regex.IsMatch(subject, @"^\[.+\]: Новое  - (\w){1,2}?(\d){2,3}");
        }
    }

    private async Task RegisterAsMonitoring(string issueNo)
    {
        Console.WriteLine("Received monitoring issue, processing...");
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);

        var registeringParameters = _config.GetSection("QueryParameters:QueryRegisterParameters").Get<string[]>();
        var assigningParameters = _config.GetSection("QueryParameters:QueryInWorkParameters").Get<string[]>();

        var isOkRegistered = await _evapi.UpdateIssue(issueNo, registeringParameters);

        if (isOkRegistered == HttpStatusCode.OK)
        {
            var isOkInWork = await _evapi.UpdateIssue(issueNo, assigningParameters);
            if (isOkInWork == HttpStatusCode.OK)
            {
                PrintNotification($"Succesfully assigned issue no. {issueNo} to first line operators", ConsoleColor.DarkGreen);
            }
        }
    }

    private async Task RegisterAsExternalIT(string issueNo)
    {
        Console.WriteLine("Received External IT issue, processing...");
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);

        var registeringParameters = _config.GetSection("QueryParameters:QueryExternalITParameters").Get<string[]>();

        if (await _evapi.UpdateIssue(issueNo, registeringParameters) == HttpStatusCode.OK)
        {
            PrintNotification($"Succesfully registered issue no. {issueNo} as an External IT issue.", ConsoleColor.DarkGreen);
        }
    }
}