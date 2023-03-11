namespace EvAutoreg.Data.Models;

public class AutoregstrarSettingsModel
{
    public string ExchangeServerUri { get; set; } = default!;
    public string ExtraViewUri { get; set; } = default!;
    public string NewIssueRegex { get; set; } = default!;
    public string IssueNoRegex { get; set; } = default!;
}