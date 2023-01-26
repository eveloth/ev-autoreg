﻿using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Mapping;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class RuleService : IRuleService
{
    private readonly IMapper _mapper;
    private readonly IMappingHelper _mappingHelper;
    private readonly IUnitofWork _unitofWork;

    public RuleService(IMapper mapper, IMappingHelper mappingHelper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _mappingHelper = mappingHelper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<Rule>> GetAll(
        int userId,
        PaginationQuery paginationQuery,
        CancellationToken cts
    )
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var rules = await _unitofWork.RuleRepository.GetAll(userId, filter, cts);
        await _unitofWork.CommitAsync(cts);

        var result = rules.Select(x => _mappingHelper.JoinIssueTypeAndField(x, cts).Result);
        return result;
    }

    public async Task<Rule> Get(int ruleId, int userId, CancellationToken cts)
    {
        var rule = await _unitofWork.RuleRepository.Get(ruleId, userId, cts);
        await _unitofWork.CommitAsync(cts);

        if (rule is null)
        {
            throw new ApiException().WithApiError(ErrorCode[6004]);
        }

        var result = await _mappingHelper.JoinIssueTypeAndField(rule, cts);
        return result;
    }

    public async Task<Rule> Add(Rule rule, CancellationToken cts)
    {
        var existingRule = await _unitofWork.RuleRepository.Get(rule.Id, rule.OwnerUserId, cts);

        if (existingRule is null)
        {
            throw new ApiException().WithApiError(ErrorCode[6004]);
        }

        var existingIssueType = await _unitofWork.IssueTypeRepository.Get(rule.IssueType.Id, cts);

        if (existingIssueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var existingIssueField = await _unitofWork.IssueFieldRepository.Get(
            rule.IssueField.Id,
            cts
        );

        if (existingIssueField is null)
        {
            throw new ApiException().WithApiError(ErrorCode[8004]);
        }

        var ruleModel = _mapper.Map<RuleModel>(rule);

        var createdRule = await _unitofWork.RuleRepository.Add(ruleModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<Rule>((createdRule, existingIssueType, existingIssueField));
        return result;
    }

    public async Task<Rule> Update(Rule rule, CancellationToken cts)
    {
        var existingRule = await _unitofWork.RuleRepository.Get(rule.Id, rule.OwnerUserId, cts);

        if (existingRule is null)
        {
            throw new ApiException().WithApiError(ErrorCode[6004]);
        }

        var existingIssueType = await _unitofWork.IssueTypeRepository.Get(rule.IssueType.Id, cts);

        if (existingIssueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var existingIssueField = await _unitofWork.IssueFieldRepository.Get(
            rule.IssueField.Id,
            cts
        );

        if (existingIssueField is null)
        {
            throw new ApiException().WithApiError(ErrorCode[8004]);
        }

        var ruleModel = _mapper.Map<RuleModel>(rule);

        var updatedRule = await _unitofWork.RuleRepository.Update(ruleModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<Rule>((updatedRule, existingIssueType, existingIssueField));
        return result;
    }

    public async Task<Rule> Delete(int ruleId, int userId, CancellationToken cts)
    {
        var existingRule = await _unitofWork.RuleRepository.Get(ruleId, userId, cts);

        if (existingRule is null)
        {
            throw new ApiException().WithApiError(ErrorCode[6004]);
        }

        var deletedRule = await _unitofWork.RuleRepository.Delete(ruleId, userId, cts);
        await _unitofWork.CommitAsync(cts);
        var result = await _mappingHelper.JoinIssueTypeAndField(deletedRule, cts);
        return result;
    }
}