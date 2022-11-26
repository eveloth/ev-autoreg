namespace EvAutoreg.Contracts.Requests;

public record RuleRequest(
    string Rule,
    int IssueTypeId,
    int IssueFieldId,
    bool IsRegex,
    bool IsNegative
);
