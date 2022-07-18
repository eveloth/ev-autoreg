using System.Xml.Linq;

namespace EVAutoreg;

class EVApiWrapper
{
    private string _username;
    private string _password;
    private string _domain;
    private HttpClient _client = new HttpClient();

    public EVApiWrapper(string username, string password, string serverAddress)
    {
        _username = username;
        _password = password;
        _domain = serverAddress;
    }

    public async Task GetIssue(string issueNo)
    {
        _client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI");

        HttpResponseMessage issue = await _client.GetAsync($"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=get&id={issueNo}");
        var issueToDisplay = await issue.Content.ReadAsStringAsync();

        Console.WriteLine(issueToDisplay);

        XDocument doc = XDocument.Parse(issueToDisplay);
        Console.WriteLine(doc);
    }
}