﻿using EvAutoreg.Api.Domain;
using EvAutoreg.Data.Models;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;

namespace EvAutoreg.Api.Mapping;

public class MappingHelper : IMappingHelper
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public MappingHelper(IUnitofWork unitofWork, IMapper mapper)
    {
        _unitofWork = unitofWork;
        _mapper = mapper;
    }

    public async Task<User> JoinUserRole(UserModel userModel, CancellationToken cts)
    {
        if (userModel.RoleId is null)
        {
            return _mapper.Map<User>(userModel);
        }

        var roleModel = await _unitofWork.RoleRepository.Get(userModel.RoleId.Value, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<User>((userModel, roleModel!));
    }

    public async Task<Issue> JoinIssueTypeAndUser(IssueModel issueModel, CancellationToken cts)
    {
        var registrar = await _unitofWork.UserRepository.GetById(issueModel.RegistrarId!.Value, cts);
        var issueType = await _unitofWork.IssueTypeRepository.Get(issueModel.IssueTypeId!.Value, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<Issue>((issueModel, registrar, issueType));
    }

    public async Task<QueryParameters> JoinIssueType(QueryParametersModel model, CancellationToken cts)
    {
        var issueType = await _unitofWork.IssueTypeRepository.Get(model.IssueTypeId, cts);
        await _unitofWork.CommitAsync(cts);

        return _mapper.Map<QueryParameters>((model, issueType));
    }
}