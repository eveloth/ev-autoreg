using System.Text.RegularExpressions;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Reflection;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;

namespace EvAutoreg.Autoregistrar.Services;

public class IssueAnalyzer : IIssueAnalyzer
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    private const RegexOptions RegexOptions =
        System.Text.RegularExpressions.RegexOptions.CultureInvariant
        | System.Text.RegularExpressions.RegexOptions.IgnoreCase;

    private readonly ILogger<IssueAnalyzer> _logger;
    private readonly ILogDispatcher<IssueAnalyzer> _logDispatcher;
    private readonly IssuePropertyInfos _issuePropertyInfos;

    public IssueAnalyzer(
        ILogger<IssueAnalyzer> logger,
        ILogDispatcher<IssueAnalyzer> logDispatcher,
        IssuePropertyInfos issuePropertyInfos
    )
    {
        _logger = logger;
        _logDispatcher = logDispatcher;
        _issuePropertyInfos = issuePropertyInfos;
    }

    public async Task<int?> AnalyzeIssue(XmlIssue issue)
    {
        var issueTypes = GlobalSettings.IssueTypes!.Where(x => x.RuleSets.Any());

        foreach (var issueType in issueTypes)
        {
            var result = await MatchesAnyRuleSet(issue, issueType.RuleSets.ToAsyncEnumerable());

            if (result.match)
            {
                return result.issueTypeId;
            }
        }

        return null;
    }

    private async Task<(bool match, int? issueTypeId)> MatchesAnyRuleSet(
        XmlIssue issue,
        IAsyncEnumerable<RuleSet> ruleSets
    )
    {
        var match = false;

        await foreach (var ruleSet in ruleSets)
        {
            match = await ruleSet.Rules
                .ToAsyncEnumerable()
                .AllAwaitAsync(async x => await MatchesARule(issue, x));

            if (!match)
            {
                continue;
            }

            return (match, ruleSet.IssueType.Id);
        }

        return (match, null);
    }

    private async Task<bool> MatchesARule(XmlIssue issue, Rule rule)
    {
        var fieldName = rule.IssueField.FieldName;
        var currentProperty = _issuePropertyInfos.XmlIssueProps.Single(x => x.Name == fieldName);
        var propertyValue = currentProperty.GetValue(issue);

        if (propertyValue is not string fieldValue)
        {
            await _logDispatcher.DispatchWarning(
                $"Parsing error occured for issue ID {issue.Id} in field name {fieldName}"
            );

            _logger.LogWarning(
                "A parsing error occured while trying to analyze issue ID {IssueId}. "
                    + "Expected type {Type} for field Issue.{Field}, got {ActualType}",
                issue.Id,
                typeof(string),
                fieldName,
                propertyValue?.GetType().Name ?? "NULL"
            );
            return false;
        }

        var result = rule switch
        {
            { IsRegex: false, IsNegative: false } => IsSubstring(rule.RuleSubstring, fieldValue),

            { IsRegex: false, IsNegative: true } => !IsSubstring(rule.RuleSubstring, fieldValue),

            { IsRegex: true, IsNegative: false } => MatchesRegexp(rule.RuleSubstring, fieldValue),

            { IsRegex: true, IsNegative: true } => !MatchesRegexp(rule.RuleSubstring, fieldValue)
        };

        return result;
    }

    private static bool IsSubstring(string substring, string source)
    {
        return source.Contains(substring, Comparison);
    }

    private static bool MatchesRegexp(string regexp, string source)
    {
        return Regex.IsMatch(source, regexp, RegexOptions);
    }
}