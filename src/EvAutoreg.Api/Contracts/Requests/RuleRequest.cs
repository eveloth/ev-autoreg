namespace EvAutoreg.Api.Contracts.Requests;

public record RuleRequest(
    string RuleSubstring,
    int IssueFieldId,
    bool IsRegex,
    bool IsNegative
);