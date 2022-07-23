using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.Net;
using static ColouredConsole;

namespace EVAutoreg;

class EVApiWrapper
{
    private readonly IConfiguration _config;
    private readonly string _domain;
    private readonly string _username;
    private readonly string _password;
    private readonly HttpClient _client = new();

    public EVApiWrapper(IConfiguration config)
    {
        _config = config;
        _domain = _config.GetValue<string>("ExtraViewCredentials:URL");
        _username = _config.GetValue<string>("ExtraViewCredentials:Username");
        _password = WebUtility.UrlEncode(_config.GetValue<string>("ExtraViewCredentials:Password"));
    }

    public async Task GetIssue(string issueNo)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        string query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=get&id={issueNo}";

        HttpResponseMessage issue = await _client.GetAsync(query);
        var issueToDisplay = await issue.Content.ReadAsStringAsync();

        if (issue.IsSuccessStatusCode && issueToDisplay.StartsWith("<?xml"))
        {
            PrintInGreen("Issue retrieved successfully.");
            
            XDocument doc = XDocument.Parse(issueToDisplay);
            Console.WriteLine(doc);
        }
        else
        {
            PrintInRed("Failed retrieveing an issue. Reason: ");
            Console.WriteLine(issueToDisplay);
        }
    }

    public async Task<HttpStatusCode> UpdateIssue(string issueNo, params string[] queryUpdateParameters)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        string query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=update&id={issueNo}";

        foreach (var param in queryUpdateParameters)
        {
            query += $"&{param}";
        }

        HttpResponseMessage res = await _client.GetAsync(query);
        var content = await res.Content.ReadAsStringAsync();

        if (res.IsSuccessStatusCode && content.StartsWith("<?xml"))
        {
            Console.WriteLine($"Issue {issueNo} successfully updated");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed updating an issue. Reason: ");
            Console.ResetColor();
            Console.WriteLine(content);

            return HttpStatusCode.BadRequest;
        }

        return HttpStatusCode.OK;
    }
}