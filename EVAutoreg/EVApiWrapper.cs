namespace EVAutoreg;

class EVApiWrapper
{
    private string _username;
    private string _password;
    private string _domain;

    public EVApiWrapper(string username, string password, string domain)
    {
        _username = username;
        _password = password;
        _domain = domain;
    }

    public async Task GetIssue(HttpClient client, int issueNo)
    {
        client.DefaultRequestHeaders.Add("User-agent", "OperatorsAPI, 0.1");

        var issueTask = client.GetAsync($"https://{_domain}/evj/ExtraView/ev_api.action?user_id={_username}&password={_password}&statevar=get&id={issueNo}");
        var issue = await issueTask;

        Console.WriteLine(issue);
    }
}