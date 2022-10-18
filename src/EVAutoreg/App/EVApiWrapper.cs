using System.Net;
using Data.Models;
using EVAutoreg.Auxiliary;
using EVAutoreg.Interfaces;
using Microsoft.Extensions.Configuration;
using static EVAutoreg.Auxiliary.PrettyPrinter;

namespace EVAutoreg.App;

public class EVApiWrapper : IEVApiWrapper
{
    private readonly string _domain;
    private readonly string _username;
    private readonly string _password;
    private readonly HttpClient _client = new();

    public EVApiWrapper(IConfiguration config)
    {
        _domain = config.GetValue<string>("ExtraViewCredentials:URL");
        _username = config.GetValue<string>("ExtraViewCredentials:Username");
        _password = WebUtility.UrlEncode(config.GetValue<string>("ExtraViewCredentials:Password"));
    }

    public async Task<XmlIssueModel?> GetIssue(string issueNo)
    {
        PrintNotification("Retrieveing the issue from EV server...", ConsoleColor.Yellow);
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        var query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=get&id={issueNo}";

        var res = await _client.GetAsync(query);
        var xmlIssueString = await res.Content.ReadAsStringAsync();

        if (res.IsSuccessStatusCode && xmlIssueString.StartsWith("<?xml"))
        {
            PrintNotification("Issue retrieved successfully.", ConsoleColor.Green);

            return XmlParser.Deserialize<XmlIssueModel>(xmlIssueString);
        }

        if(xmlIssueString.Contains("AREA TITLE"))
        {
            PrintNotification("Issue was marked as SPAM.", ConsoleColor.Green);
            return null;
        }

        PrintNotification("Failed retrieveing an issue. Reason: ", ConsoleColor.Red);
        Console.WriteLine(xmlIssueString);
        return null;
    }

    public async Task<HttpStatusCode> UpdateIssue(string issueNo, params string[] queryUpdateParameters)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        var query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=update&id={issueNo}";

        query = queryUpdateParameters.Aggregate(query, (current, param) => current + $"&{param}");

        var res = await _client.GetAsync(query);
        var content = await res.Content.ReadAsStringAsync();

        if (res.IsSuccessStatusCode && content.Contains("updated", StringComparison.InvariantCultureIgnoreCase))
        {
            PrintNotification("EV server: " + content, ConsoleColor.Green);
        }
        else
        {
            PrintNotification("Failed updating an issue. Reason: ", ConsoleColor.Red);
            Console.WriteLine(content);
            
            return HttpStatusCode.BadRequest;
        }

        return HttpStatusCode.OK;
    }
}