using System.Text.RegularExpressions;

namespace EvAutoreg.Autoregistrar.Domain;

public record AutoregistrarSettings
{
    public required string ExchangeServerUri { get; set; }
    public required string ExtraViewUri { get; set; }
    public required Regex NewIssueRegex { get; set; }
    public required Regex IssueNoRegex { get; set; }
}