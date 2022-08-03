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

    public MailEventListener(IConfiguration config, IEVApiWrapper evapi)
    {
        _config = config;
        _evapi = evapi;
    }

    public async Task ProcessEvent(EmailMessage email)
    {
        var subject = email.Subject;
        var body = email.Body.Text;
        
        if (IsRegisteringNeeded(subject, body))
        {
            Console.WriteLine("Received monitoring issue, processing...");
            try
            {
                var issueNo = Regex.Match(subject, @"^\[.+(\d{6})\]").Groups[1].Value;

                Console.Write("Issue No. to process: ");
                PrintNotification(issueNo, ConsoleColor.Magenta);

                //await AssignIssueToFirstLineOperators(issueNo);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                PrintNotification($"Failed assigning an issue,\n{e.Message}", ConsoleColor.Red);
            }
        }
    }

    private bool IsRegisteringNeeded(string subject, string body)
    {
        var subjectRules = _config.GetSection("MailAnalysisRules:SubjectRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var bodyRules = _config.GetSection("MailAnalysisRules:BodyRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var subjectNegativeRules = _config.GetSection("MailAnalysisRules:SubjectNegativeRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();
        var bodyNegativeRules = _config.GetSection("MailAnalysisRules:BodyNegativeRules").Get<string[]>()
            ?? Array.Empty<string>().ToArray();

        return (subjectRules.Any(s => subject.Contains(s)) && !subjectNegativeRules.Any(s => body.Contains(s))) ||
            (bodyRules.Any(s => body.Contains(s)) && !bodyNegativeRules.Any(s => body.Contains(s))) ||
            Regex.IsMatch(subject, @"^\[.+\]: Новое.{1,4}\d{6}(?:.{0,2})?$");
    }

    private async Task AssignIssueToFirstLineOperators(string issueNumber)
    {   
        var registeringParameters = _config.GetSection("QueryParameters:QueryRegisterParameters").Get<string[]>();
        var assigningParameters = _config.GetSection("QueryParameters:QueryInWorkParameters").Get<string[]>();
        
        var isOkRegistered = await _evapi.UpdateIssue(issueNumber, registeringParameters);

        if (isOkRegistered == HttpStatusCode.OK)
        {
            var isOkInWork = await _evapi.UpdateIssue(issueNumber, assigningParameters);
            if (isOkInWork == HttpStatusCode.OK)
            {
                Console.WriteLine($"Succesfully assigned issue no. {issueNumber} to first line operators");
            }
        }
    }
}