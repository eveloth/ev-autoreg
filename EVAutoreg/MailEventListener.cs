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
        var content = email.Body.Text;
        var issueNo = Regex.Match(subject, @"^\[.+(\d{6})\]").Groups[1].Value;

        Console.Write("Issue No. to process: ");
        PrintNotification(issueNo, ConsoleColor.Magenta);


        if (subject.Contains("is unreachable") ||
            subject.Contains("unavailable by", StringComparison.InvariantCultureIgnoreCase) ||
            subject.Contains("is not OK") ||
            subject.Contains("has just been restarted") ||
            subject.Contains("Free disk space") ||
            subject.Contains("are not responding") ||
            subject.Contains("ping response time") ||
            subject.Contains("is above critical threshold") ||
            subject.Contains("does not send any pings") ||
            subject.Contains("high utilization") ||
            subject.Contains("more than") ||
            content.Contains("на PROBLEM") ||
            Regex.IsMatch(subject, @"^\[.+\]: Новое.{1,4}\d{6}(?:.{0,2})?$"))
        {
            Console.WriteLine("Received monitoring issue, processing...");
            try
            {
                //await AssignIssueToFirstLineOperators(issueNo);
                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                PrintNotification($"Failed assigning an issue,\n{e.Message}", ConsoleColor.Red);
            }
        }
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