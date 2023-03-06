using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Exceptions;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;
using static EvAutoreg.Api.Errors.ErrorCodes;

namespace EvAutoreg.Api.Services;

public class RuleSetService : IRuleSetService
{
    private readonly IUnitofWork _unitofWork;
    private readonly IMapper _mapper;

    public RuleSetService(IUnitofWork unitofWork, IMapper mapper)
    {
        _unitofWork = unitofWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RuleSet>> GetAllForIssueType(
        int ownerUserId,
        int issueTypeId,
        CancellationToken cts
    )
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(issueTypeId, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var ruleSetModels = await _unitofWork.RuleSetRepository.GetAllForIssueType(
            ownerUserId,
            issueTypeId,
            cts
        );
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<IEnumerable<RuleSet>>(ruleSetModels);
    }

    public async Task<IEnumerable<RuleSet>> Add(RuleSet ruleSet, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(ruleSet.IssueType.Id, cts);

        if (issueType is null)
        {
            throw new ApiException().WithApiError(ErrorCode[7004]);
        }

        var ruleSetModel = _mapper.Map<RuleSetModel>(ruleSet);
        var updatedRuleSets = await _unitofWork.RuleSetRepository.Add(ruleSetModel, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<IEnumerable<RuleSet>>(updatedRuleSets);
    }

    public async Task<IEnumerable<RuleSet>> Delete(
        int ownerUserId,
        int ruleSetId,
        CancellationToken cts
    )
    {
        var ruleSetExists = await _unitofWork.RuleSetRepository.DoesRuleSetExist(
            ownerUserId,
            ruleSetId,
            cts
        );

        if (!ruleSetExists)
        {
            throw new ApiException().WithApiError(ErrorCode[6014]);
        }

        var updatedRuleSets = await _unitofWork.RuleSetRepository.Delete(
            ownerUserId,
            ruleSetId,
            cts
        );
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<IEnumerable<RuleSet>>(updatedRuleSets);
    }

    public async Task<RuleSet> AddEntry(int ownerUserId, Rule rule, CancellationToken cts)
    {
        var ruleSetExists = await _unitofWork.RuleSetRepository.DoesRuleSetExist(
            ownerUserId,
            rule.RuleSetId,
            cts
        );

        if (!ruleSetExists)
        {
            throw new ApiException().WithApiError(ErrorCode[6014]);
        }

        var issueFiledExists = await _unitofWork.IssueFieldRepository.DoesExist(
            rule.IssueField.Id,
            cts
        );

        if (!issueFiledExists)
        {
            throw new ApiException().WithApiError(ErrorCode[8004]);
        }

        var ruleModel = _mapper.Map<RuleModel>(rule);
        var updatedRuleSet = await _unitofWork.RuleSetRepository.AddEntry(ruleModel, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<RuleSet>(updatedRuleSet);
    }

    public async Task<RuleSet> UpdateEntry(int ownerUserId, Rule rule, CancellationToken cts)
    {
        var ruleSetExists = await _unitofWork.RuleSetRepository.DoesRuleSetExist(
            ownerUserId,
            rule.RuleSetId,
            cts
        );

        if (!ruleSetExists)
        {
            throw new ApiException().WithApiError(ErrorCode[6014]);
        }

        var existingRule = await _unitofWork.RuleSetRepository.GetEntry(
            rule.RuleSetId,
            rule.Id,
            cts
        );

        if (existingRule is null)
        {
            throw new ApiException().WithApiError(ErrorCode[6004]);
        }

        var issueFiledExists = await _unitofWork.IssueFieldRepository.DoesExist(
            rule.IssueField.Id,
            cts
        );

        if (!issueFiledExists)
        {
            throw new ApiException().WithApiError(ErrorCode[8004]);
        }

        var ruleModel = _mapper.Map<RuleModel>(rule);
        var updatedRuleSet = await _unitofWork.RuleSetRepository.UpdateEntry(ruleModel, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<RuleSet>(updatedRuleSet);
    }

    public async Task<RuleSet> DeleteEntry(
        int ownerUserId,
        int ruleSetId,
        int ruleId,
        CancellationToken cts
    )
    {
        var ruleSetExists = await _unitofWork.RuleSetRepository.DoesRuleSetExist(
            ownerUserId,
            ruleSetId,
            cts
        );

        if (!ruleSetExists)
        {
            throw new ApiException().WithApiError(ErrorCode[6014]);
        }

        var ruleExists = await _unitofWork.RuleSetRepository.GetEntry(ruleSetId, ruleId, cts);

        if (ruleExists is null)
        {
            throw new ApiException().WithApiError(ErrorCode[6004]);
        }

        var updatedRuleSet = await _unitofWork.RuleSetRepository.DeleteEntry(ruleId, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<RuleSet>(updatedRuleSet);
    }
}