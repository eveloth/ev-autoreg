namespace EvAutoreg.Api.Contracts.Dto;

public class AutoregistrarSettingsDto
{
    public required string ExchangeServerUri { get; set; }
    public required string ExtraViewUri { get; set; }
    public required string NewIssueRegex { get; set; }
    public required string IssueNoRegex { get; set; }
}