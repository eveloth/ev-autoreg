using System.Text.RegularExpressions;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Reflection;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Extensions;

namespace EvAutoreg.Autoregistrar.Services;

public class IssueAnalyzer : IIssueAnalyzer
{
    private readonly ILogger<IssueAnalyzer> _logger;
    private readonly ILogDispatcher<IssueAnalyzer> _logDispatcher;
    private readonly IssuePropertyInfos _issuePropertyInfos;

    public IssueAnalyzer(
        ILogDispatcher<IssueAnalyzer> logDispatcher,
        ILogger<IssueAnalyzer> logger,
        IssuePropertyInfos issuePropertyInfos
    )
    {
        _logDispatcher = logDispatcher;
        _logger = logger;
        _issuePropertyInfos = issuePropertyInfos;
    }

    public async Task<int?> AnalyzeIssue(XmlIssue issue)
    {
        var issueFields = GlobalSettings.IssueFields!;

        foreach (var field in issueFields)
        {
            if (field.Rules.No())
            {
                continue;
            }

            var currentProperty = _issuePropertyInfos.XmlIssueProps.Single(
                x => x.Name == field.FieldName
            );
            var propertyValue = currentProperty.GetValue(issue);

            if (propertyValue is not string stringToAnalyze)
            {
                await _logDispatcher.Log(
                    $"Parsing error occured for issue ID {issue.Id} in field name {field.FieldName}"
                );

                _logger.LogWarning(
                    "A parsing error occured while trying to analyze issue ID {IssueId}. "
                        + "Expected type {Type} for field Issue.{Field}, got {ActualType}",
                    issue.Id,
                    typeof(string),
                    field.FieldName,
                    propertyValue?.GetType().Name ?? "NULL"
                );
                continue;
            }

            var rules = field.Rules;

            var issueTypeId = ComputeIssueType(stringToAnalyze, rules);

            if (issueTypeId is not null)
            {
                return issueTypeId;
            }
        }

        return null;
    }

    private static int? ComputeIssueType(string stringToAnalyze, IEnumerable<Rule> rules)
    {
        var rulesByIssueType = rules.GroupBy(x => x.IssueTypeId);

        foreach (var ruleSet in rulesByIssueType)
        {
            var issueTypeId = ruleSet.Key;

            if (
                MatchesARule(ruleSet, stringToAnalyze)
                    && DoesntMatchANegativeRule(ruleSet, stringToAnalyze)
                || MatchesARegExp(ruleSet, stringToAnalyze)
                    && DoesntMatchANegativeRegExp(ruleSet, stringToAnalyze)
            )
            {
                return issueTypeId;
            }
        }

        return null;
    }

    private static bool MatchesARule(IEnumerable<Rule> ruleSet, string input)
    {
        return ruleSet
            .Where(x => x is { IsNegative: false, IsRegex: false })
            .Select(x => x.RuleSubstring)
            .Any(x => input.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }

    private static bool DoesntMatchANegativeRule(IEnumerable<Rule> ruleSet, string input)
    {
        return !ruleSet
            .Where(x => x is { IsNegative: true, IsRegex: false })
            .Select(x => x.RuleSubstring)
            .Any(x => input.Contains(x, StringComparison.InvariantCultureIgnoreCase));
    }

    private static bool MatchesARegExp(IEnumerable<Rule> regExpCollection, string input)
    {
        return regExpCollection
            .Where(x => x is { IsNegative: false, IsRegex: true })
            .Select(x => x.RuleSubstring)
            .Any(x => Regex.IsMatch(input, x));
    }

    private static bool DoesntMatchANegativeRegExp(IEnumerable<Rule> regExpCollection, string input)
    {
        return !regExpCollection
            .Where(x => x is { IsNegative: true, IsRegex: true })
            .Select(x => x.RuleSubstring)
            .Any(x => Regex.IsMatch(input, x));
    }
}