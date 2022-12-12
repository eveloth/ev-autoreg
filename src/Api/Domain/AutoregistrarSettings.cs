namespace Api.Domain;

public class AutoregistrarSettings
{
    public string ExchangeServerUri { get; set; }
    public string ExtraViewUri { get; set; }
    public string NewIssueRegex { get; set; }
    public string IssueNoRegex { get; set; }
}