using System.Net;
using System.Text.RegularExpressions;
using Data.Data;
using Data.Extensions;
using EVAutoregConsole.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using static EVAutoregConsole.Auxiliary.PrettyPrinter;
using Task = System.Threading.Tasks.Task;

// ReSharper disable InconsistentNaming

namespace EVAutoregConsole.App;

public class MailEventListener : IMailEventListener
{
    private readonly IConfiguration _config;
    private readonly IEVApiWrapper _evapi;
    private readonly Rules _rules;
    private readonly IIssueData _issueData;

    private enum IssueType
    {
        Monitoring,
        ExternalIT,
        Spam,
        None
    }

    public MailEventListener(
        IConfiguration config,
        IEVApiWrapper evapi,
        Rules rules,
        IIssueData issueData
    )
    {
        _config = config;
        _evapi = evapi;
        _rules = rules;
        _issueData = issueData;
    }

    public async Task ProcessEvent(EmailMessage email)
    {
        var subject = email.Subject;
        var body = email.Body.Text;
        var issueNo = Regex.Match(subject, _rules.RegexIssueNo).Groups[1].Value;

        var action = IdentifyIssueType(subject, body, _rules) switch
        {
            IssueType.Spam => RegisterAsSpam(issueNo),
            IssueType.Monitoring => RegisterAsMonitoring(issueNo),
            IssueType.ExternalIT => RegisterAsExternalIT(issueNo),
            _
                => Task.Run(
                    () =>
                        PrintNotification(
                            "The issue does not match any rules, skipping",
                            ConsoleColor.Blue
                        )
                )
        };

        await action;
    }

    private IssueType IdentifyIssueType(string subject, string body, Rules rules)
    {
        return (subject, body) switch
        {
            _ when IsSpam(subject, body) => IssueType.Spam,
            _ when IsMonitoring(subject, body) => IssueType.Monitoring,
            _ when IsExternalIT(subject, body) => IssueType.ExternalIT,
            _ => IssueType.None
        };

        bool IsSpam(string subj, string bdy)
        {
            return rules.SpamSubjectRules.Any(subj.Contains)
                || rules.SpamBodyRules.Any(bdy.Contains);
        }

        bool IsMonitoring(string subj, string bdy)
        {
            return rules.SubjectRules.Any(subj.Contains)
                    && !rules.SubjectNegativeRules.Any(subj.Contains)
                || rules.BodyRules.Any(bdy.Contains) && !rules.BodyNegativeRules.Any(bdy.Contains)
                || Regex.IsMatch(subj, _rules.RegexMonitoring);
        }

        bool IsExternalIT(string subj, string bdy)
        {
            return rules.ExternalITSubjectRules.Any(subj.Contains)
                || rules.ExternalITBodyRules.Any(bdy.Contains)
                || Regex.IsMatch(subj, _rules.RegexExternalIT);
        }
    }

    private async Task RegisterAsSpam(string issueNo)
    {
        PrintNotification("Received SPAM issue, processing...", ConsoleColor.Blue);
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);
        Console.WriteLine();

        var xmlIssue = await _evapi.GetIssue(issueNo);
        var issue = xmlIssue!.ConvertToSqlModel();

        issue.Status = "SPAM";

        try
        {
            await _issueData.UpsertIssue(issue);
            PrintNotification($"Added issue no {issueNo} to database", ConsoleColor.DarkCyan);
        }
        catch (Exception e)
        {
            PrintNotification(
                $"Failed to commit transaction, reason: {e.Message}",
                ConsoleColor.Red
            );
        }

        var registeringParameters = _config
            .GetSection("QueryParameters:QuerySpamParameters")
            .Get<string[]>();

        if (await _evapi.UpdateIssue(issueNo, registeringParameters) == HttpStatusCode.OK)
        {
            PrintNotification(
                $"Successfully registered issue no. {issueNo} as SPAM issue.\n",
                ConsoleColor.DarkGreen
            );
        }
    }

    private async Task RegisterAsMonitoring(string issueNo)
    {
        PrintNotification("Received monitoring issue, processing...", ConsoleColor.Blue);
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);
        Console.WriteLine();

        var registeringParameters = _config
            .GetSection("QueryParameters:QueryRegisterParameters")
            .Get<string[]>();
        var assigningParameters = _config
            .GetSection("QueryParameters:QueryInWorkParameters")
            .Get<string[]>();

        var isOkRegistered = await _evapi.UpdateIssue(issueNo, registeringParameters);

        if (isOkRegistered == HttpStatusCode.OK)
        {
            var isOkInWork = await _evapi.UpdateIssue(issueNo, assigningParameters);
            if (isOkInWork == HttpStatusCode.OK)
            {
                PrintNotification(
                    $"Successfully assigned issue no. {issueNo} to first line operators.",
                    ConsoleColor.DarkGreen
                );

                var xmlIssue = await _evapi.GetIssue(issueNo);
                var issue = xmlIssue!.ConvertToSqlModel();

                try
                {
                    PrintNotification("Initiating database transaction...", ConsoleColor.Yellow);
                    await _issueData.UpsertIssue(issue);
                    PrintNotification(
                        $"Issue no. {issueNo} id added to the database.\n",
                        ConsoleColor.DarkCyan
                    );
                }
                catch (Exception e)
                {
                    PrintNotification(
                        $"Failed to commit transaction, reason: {e.Message}",
                        ConsoleColor.Red
                    );
                }
            }
        }
    }

    private async Task RegisterAsExternalIT(string issueNo)
    {
        PrintNotification("Received External IT issue, processing...", ConsoleColor.Blue);
        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);
        Console.WriteLine();

        var registeringParameters = _config
            .GetSection("QueryParameters:QueryExternalITParameters")
            .Get<string[]>();

        if (await _evapi.UpdateIssue(issueNo, registeringParameters) == HttpStatusCode.OK)
        {
            PrintNotification(
                $"Successfully registered issue no. {issueNo} as an External IT issue.\n",
                ConsoleColor.DarkGreen
            );
        }
    }
}
