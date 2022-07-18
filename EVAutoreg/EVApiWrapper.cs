using System.Xml.Linq;

namespace EVAutoreg;

class EVApiWrapper
{
    private readonly string _username;
    private readonly string _password;
    private readonly string _domain;
    private readonly HttpClient _client = new();

    public EVApiWrapper(string serverAddress, string username, string password)
    {
        _domain = serverAddress;
        _username = username;
        _password = password;
    }

    public async Task GetIssue(string issueNo)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        HttpResponseMessage issue = await _client.GetAsync($"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=get&id={issueNo}");

        if (issue.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Issue retrieved successfully.");
            Console.ResetColor();
        }
        var issueToDisplay = await issue.Content.ReadAsStringAsync();

        XDocument doc = XDocument.Parse(issueToDisplay);
        Console.WriteLine(doc);
    }

    public async Task<HttpResponseMessage> UpdateIssue(string issueNo, params string[] queryUpdateParameters)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        string query = $"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=update&id={issueNo}";

        foreach (var param in queryUpdateParameters)
        {
            query += $"&{param}";
        }

        HttpResponseMessage res = await _client.GetAsync(query);

        if (res.IsSuccessStatusCode)
        {
            Console.WriteLine($"Issue {issueNo} successfully updated");
        }

        return res;
    }
}