namespace Api.Contracts.Requests;

public record MailAnalysisRulesRequest(string NewIssueRegex, string IssueNoRegex);
