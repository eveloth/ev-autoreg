﻿using Api.Contracts;
using Api.Domain;
using Api.Exceptions;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using static Api.Errors.ErrorCodes;

namespace Api.Services;

public class IssueTypeService : IIssueTypeService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public IssueTypeService(IMapper mapper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<IssueType>> GetAll(PaginationQuery paginationQuery, CancellationToken cts)
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var issueTypes = await _unitofWork.IssueTypeRepository.GetAll(filter, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IEnumerable<IssueType>>(issueTypes);
        return result;
    }

    public async Task<IssueType> Get(int id, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(id, cts);
        await _unitofWork.CommitAsync(cts);

        if (issueType is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[7004]);
            throw e;
        }

        var result = _mapper.Map<IssueType>(issueType);
        return result;
    }

    public async Task<IssueType> Add(IssueType type, CancellationToken cts)
    {
        var issueTypeNameTaken = await _unitofWork.IssueTypeRepository.DoesExist(type.IssueTypeName, cts);

        if (issueTypeNameTaken)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[7001]);
            throw e;
        }

        var issueTypeModel = _mapper.Map<IssueTypeModel>(type);
        var createdIssueType = await _unitofWork.IssueTypeRepository.Add(issueTypeModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IssueType>(createdIssueType);
        return result;
    }

    public async Task<IssueType> Rename(IssueType type, CancellationToken cts)
    {
        var issueTypeNameTaken = await _unitofWork.IssueTypeRepository.DoesExist(type.IssueTypeName, cts);

        if (issueTypeNameTaken)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[7001]);
            throw e;
        }

        var issueTypeModel = _mapper.Map<IssueTypeModel>(type);
        var updatedIssueType = await _unitofWork.IssueTypeRepository.ChangeName(issueTypeModel, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IssueType>(updatedIssueType);
        return result;
    }

    public async Task<IssueType> Delete(int id, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(id, cts);

        if (issueType is null)
        {
            var e = new ApiException();
            e.Data.Add("ApiError", ErrorCode[7004]);
            throw e;
        }

        var deletedIssueType = await _unitofWork.IssueTypeRepository.Delete(id, cts);
        await _unitofWork.CommitAsync(cts);
        var result = _mapper.Map<IssueType>(deletedIssueType);
        return result;
    }
}