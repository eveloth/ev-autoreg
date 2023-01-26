namespace EvAutoreg.Api.Domain;

public class AutoregistrarSettings
{
#pragma warning disable CS8618
    public string ExchangeServerUri { get; set; }
    public string ExtraViewUri { get; set; }
    public string NewIssueRegex { get; set; }
    public string IssueNoRegex { get; set; }
#pragma warning restore CS8618
}