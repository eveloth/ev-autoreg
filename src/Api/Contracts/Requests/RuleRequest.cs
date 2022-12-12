namespace Api.Contracts.Requests;

public record RuleRequest(
    string RuleSubstring,
    int IssueTypeId,
    int IssueFieldId,
    bool IsRegex,
    bool IsNegative
);