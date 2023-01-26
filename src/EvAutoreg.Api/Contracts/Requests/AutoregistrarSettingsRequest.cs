namespace EvAutoreg.Api.Contracts.Requests;

public record AutoregistrarSettingsRequest(
    string ExchangeServerUri,
    string ExtraViewUri,
    string NewIssueRegex,
    string IssueNoRegex
);