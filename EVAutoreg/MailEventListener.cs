using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using static EVAutoreg.PrettyPrinter;
using Task = System.Threading.Tasks.Task;
// ReSharper disable InconsistentNaming

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
        Spam,
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
        var issueNo = Regex.Match(subject, _rules.RegexIssueNo).Groups[1].Value;

        var action = IdentifyIssueType(subject, body, _rules) switch
        {
            IssueType.Spam       => RegisterAsSpam(issueNo),
            IssueType.Monitoring => RegisterAsMonitoring(issueNo),
            IssueType.ExternalIT => RegisterAsExternalIT(issueNo),
            _                    => Task.CompletedTask
        };

        await action;
    }

    private IssueType IdentifyIssueType(string subject, string body, Rules rules)
    {
        return (subject, body) switch
        {
            _ when IsSpam(subject)                     => IssueType.Spam,
            _ when IsMonitoring(subject, body) => IssueType.Monitoring,
            _ when IsExternalIT(subject, body) => IssueType.ExternalIT,
            _ => IssueType.None

        };

        bool IsSpam(string subj)
        {
            return rules.SpamRules.Any(subj.Contains);
        }

        bool IsMonitoring(string subj, string bdy)
        {
            return rules.SubjectRules.Any(subj.Contains) && !rules.SubjectNegativeRules.Any(subj.Contains) ||
                   rules.BodyRules.Any(bdy.Contains) && !rules.BodyNegativeRules.Any(bdy.Contains) ||
                   Regex.IsMatch(subj, _rules.RegexMonitoring);
        }

        bool IsExternalIT(string subj, string bdy)
        {
            return rules.ExternalITSubjectRules.Any(subj.Contains) ||
                   rules.ExternalITBodyRules.Any(bdy.Contains) ||
                   Regex.IsMatch(subj, _rules.RegexExternalIT);
        }
    }

    private async Task RegisterAsSpam(string issueNo)
    {
        PrintNotification("Received SPAM issue, processing...", ConsoleColor.Blue);
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);

        var registeringParameters = _config.GetSection("QueryParameters:QuerySpamParameters").Get<string[]>();

        if (await _evapi.UpdateIssue(issueNo, registeringParameters) == HttpStatusCode.OK)
        {
            PrintNotification($"Successfully registered issue no. {issueNo} as SPAM issue.", ConsoleColor.DarkGreen);
        }
    }

    private async Task RegisterAsMonitoring(string issueNo)
    {
        PrintNotification("Received monitoring issue, processing...", ConsoleColor.Blue);
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
                PrintNotification($"Successfully assigned issue no. {issueNo} to first line operators", ConsoleColor.DarkGreen);
            }
        }
    }

    private async Task RegisterAsExternalIT(string issueNo)
    {
        PrintNotification("Received External IT issue, processing...", ConsoleColor.Blue);
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);

        var registeringParameters = _config.GetSection("QueryParameters:QueryExternalITParameters").Get<string[]>();

        if (await _evapi.UpdateIssue(issueNo, registeringParameters) == HttpStatusCode.OK)
        {
            PrintNotification($"Successfully registered issue no. {issueNo} as an External IT issue.", ConsoleColor.DarkGreen);
        }
    }
}