namespace DataAccessLibrary.Models;

public class AutoregstrarSettingsModel
{
#pragma warning disable CS8618

    public int Id { get; set; }
    public string ExchangeServerUri { get; set; }
    public string ExtraViewUri { get; set; }
    public string NewIssueRegex { get; set; }
    public string IssueNoRegex { get; set; }
    
#pragma warning restore
}