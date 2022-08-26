using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using static EVAutoreg.PrettyPrinter;
using Task = System.Threading.Tasks.Task;
// ReSharper disable InconsistentNaming

namespace EVAutoreg;

public class MailEventListener : IMailEventListener
{
    private readonly IConfiguration _config;
    private readonly IEVApiWrapper _evapi;

    private enum IssueType
    {
        Monitoring,
        ExternalIT,
        None
    }

    public MailEventListener(IConfiguration config, IEVApiWrapper evapi)
    {
        _config = config;
        _evapi = evapi;
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

    private static IssueType IdentifyIssueType(string subject, string body, Rules rules)
    {
        return (subject, body) switch
        {
            _ when IsMonitoring(subject, body) => IssueType.Monitoring,
            _ when IsExternalIT(subject, body) => IssueType.ExternalIT,
            _ => IssueType.None

        };

        bool IsMonitoring(string subj, string bdy)
        {
            return rules.SubjectRules.Any(subj.Contains) && !rules.SubjectNegativeRules.Any(subj.Contains) ||
                   rules.BodyRules.Any(bdy.Contains) && !rules.BodyNegativeRules.Any(bdy.Contains) ||
                   Regex.IsMatch(subj, @"^\[.+\]: Новое.{1,4}\d{6}(?:.{0,2})?$");
        }

        bool IsExternalIT(string subj, string bdy)
        {
            return rules.ExternalITSubjectRules.Any(subj.Contains) ||
                   rules.ExternalITBodyRules.Any(bdy.Contains) ||
                   Regex.IsMatch(subj, @"^\[.+\]: Новое  - (\w){1,2}?(\d){2,3}");
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
                PrintNotification($"Successfully assigned issue no. {issueNo} to first line operators", ConsoleColor.DarkGreen);
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
            PrintNotification($"Successfully registered issue no. {issueNo} as an External IT issue.", ConsoleColor.DarkGreen);
        }
    }
}