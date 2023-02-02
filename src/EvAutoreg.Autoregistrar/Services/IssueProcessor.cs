﻿using System.Net;
using System.Text.RegularExpressions;
using EvAutoreg.Autoregistrar.Apis;
using EvAutoreg.Autoregistrar.Domain;
using EvAutoreg.Autoregistrar.Services.Interfaces;
using EvAutoreg.Autoregistrar.Settings;
using EvAutoreg.Autoregistrar.State;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;

namespace EvAutoreg.Autoregistrar.Services;

public class IssueProcessor : IIssueProcessor
{
    private readonly IMapper _mapper;
    private readonly IEvApi _evapi;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogDispatcher<IssueProcessor> _logDispatcher;

    public IssueProcessor(
        IMapper mapper,
        IEvApi evapi,
        IServiceScopeFactory scopeFactory,
        ILogDispatcher<IssueProcessor> logDispatcher
    )
    {
        _mapper = mapper;
        _evapi = evapi;
        _scopeFactory = scopeFactory;
        _logDispatcher = logDispatcher;
    }

    public async Task ProcessEvent(string issueNo)
    {
        var evResponse = await _evapi.GetIssue(issueNo);

        if (evResponse.StatusCode != HttpStatusCode.OK)
        {
            await _logDispatcher.Log("Couldn't retrieve an issue from EV server");
            return;
        }

        var xmlIssue = evResponse.Content!;
        var issueFields = GlobalSettings.IssueFields!;

        foreach (var field in issueFields)
        {
            //We don't want to use reflection if in the end every rule check will be false
            //because there are no rules associated with this issue field, e.g. with issue Author etc.
            if (field.Rules.Count < 1)
            {
                continue;
            }

            var value = xmlIssue.GetType().GetProperty(field.FieldName)!.GetValue(xmlIssue);
            var rules = field.Rules;

            if (value is not string str)
            {
                await _logDispatcher.Log(
                    $"Parsing error occured for issue ID {issueNo} in field name {field.FieldName}"
                );

                continue;
            }

            var rulesByIssueType = rules.GroupBy(x => x.IssueTypeId);

            foreach (var ruleSet in rulesByIssueType)
            {
                var issueTypeId = ruleSet.Key;

                if (
                    MatchesARule(ruleSet, str) && DoesntMatchANegativeRule(ruleSet, str)
                    || MatchesARegExp(ruleSet, str) && DoesntMatchANegativeRegExp(ruleSet, str)
                )
                {
                    await UpdateAndSaveIssue(issueTypeId, issueNo);
                    return;
                }
            }
        }
    }

    //This class's cognitive complexity is higher than desired, but as for now here we have an external infrastructure bottleneck;
    //the Extra View server has it's own set of rules on whether to consider issue properly 'registered'.
    //There rules differ from company to company, so we needed to ensure we reach at least second-level depth of 'updating':
    //the firts update changes issue status to 'regisered', and the second (if needed) -- to 'in work'.
    //If your issue updating rules are standartised enough, you might not need this second update, but it is there in case somebody needs it.
    // TODO: make cascade updates, i.e. give users the ability to create many query parameters entities for each issue type and enumerate them
    //UPD Made cascade updates
    private async Task UpdateAndSaveIssue(int issueTypeId, string issueNo)
    {
        var loadedIssueType = GlobalSettings.IssueTypes!.First(x => x.Id == issueTypeId);
        var queryParameters = loadedIssueType.QueryParameters;

        await _logDispatcher.Log($"Registering an issue as: {loadedIssueType.IssueTypeName}");

        await UpdateIssue(queryParameters, issueTypeId, issueNo);
    }

    private async Task UpdateIssue(
        IEnumerable<QueryParameters> queryParameters,
        int issueTypeId,
        string issueNo
    )
    {
        var sortedQueryParameters = queryParameters.OrderBy(x => x.ExecutionOrder);

        foreach (var qp in sortedQueryParameters)
        {
            var queryString = new[]
            {
                qp.WorkTime,
                qp.Status,
                qp.AssignedGroup ?? string.Empty,
                qp.RequestType ?? string.Empty
            };

            var response = await _evapi.UpdateIssue(issueNo, queryString);

            if (response.StatusCode is HttpStatusCode.OK)
            {
                await InsertIssueIntoDatabase(issueNo, issueTypeId);
                await _logDispatcher.Log($"Inserted issue ID {issueNo} into the database");
            }
            else
            {
                await _logDispatcher.Log(
                    $"Failed to update issue ID {issueNo}, EV server response was: {response.Content}"
                );
            }
        }
    }

    private async Task InsertIssueIntoDatabase(string issueNo, int issueType)
    {
        var xmlIssueResponse = await _evapi.GetIssue(issueNo);

        if (xmlIssueResponse.StatusCode == HttpStatusCode.OK)
        {
            var issue = _mapper.Map<IssueModel>(xmlIssueResponse.Content!);
            issue.IssueTypeId = issueType;
            issue.RegistrarId = StateManager.GetOperator();

            using var scope = _scopeFactory.CreateScope();
            var unitofWork = scope.ServiceProvider.GetService<IUnitofWork>();
            await unitofWork!.IssueRepository.Upsert(issue, CancellationToken.None);

            await unitofWork.CommitAsync(CancellationToken.None);

            await _logDispatcher.Log($"Issue ID {issue.Id} was updated");
        }
        else
        {
            await _logDispatcher.Log("Couldn't retrieve an issue from EV server");
        }
    }

    private static bool MatchesARule(IEnumerable<Rule> ruleSet, string input)
    {
        return ruleSet
            .Where(x => x is { IsNegative: false, IsRegex: false })
            .Select(x => x.RuleSubstring)
            .Any(input.Contains);
    }

    private static bool DoesntMatchANegativeRule(IEnumerable<Rule> ruleSet, string input)
    {
        return !ruleSet
            .Where(x => x is { IsNegative: true, IsRegex: false })
            .Select(x => x.RuleSubstring)
            .Any(input.Contains);
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