using Microsoft.Extensions.Configuration;
using System.Net;
using System.Xml.Linq;
using static EVAutoreg.PrettyPrinter;

namespace EVAutoreg;

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

    public async Task GetIssue(string issueNo)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        var query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=get&id={issueNo}";

        var issue = await _client.GetAsync(query);
        var issueToDisplay = await issue.Content.ReadAsStringAsync();

        if (issue.IsSuccessStatusCode && issueToDisplay.StartsWith("<?xml"))
        {
            PrintNotification("Issue retrieved successfully.", ConsoleColor.Green);
            
            var doc = XDocument.Parse(issueToDisplay);
            Console.WriteLine(doc);
        }
        else
        {
            PrintNotification("Failed retrieveing an issue. Reason: ", ConsoleColor.Red);
            Console.WriteLine(issueToDisplay);
        }
    }

    public async Task<HttpStatusCode> UpdateIssue(string issueNo, params string[] queryUpdateParameters)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        string query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=update&id={issueNo}";

        query = queryUpdateParameters.Aggregate(query, (current, param) => current + $"&{param}");

        var res = await _client.GetAsync(query);
        var content = await res.Content.ReadAsStringAsync();

        if (res.IsSuccessStatusCode && content.Contains("updated", StringComparison.InvariantCultureIgnoreCase))
        {
            PrintNotification(content, ConsoleColor.Green);
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