﻿using EvAutoreg.Api.Contracts;
using EvAutoreg.Api.Domain;
using EvAutoreg.Api.Services.Interfaces;
using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Repository.Interfaces;
using MapsterMapper;

namespace EvAutoreg.Api.Services;

public class IssueFieldService : IIssueFieldService
{
    private readonly IMapper _mapper;
    private readonly IUnitofWork _unitofWork;

    public IssueFieldService(IMapper mapper, IUnitofWork unitofWork)
    {
        _mapper = mapper;
        _unitofWork = unitofWork;
    }

    public async Task<IEnumerable<IssueField>> GetAll(PaginationQuery paginationQuery, CancellationToken cts)
    {
        var filter = _mapper.Map<PaginationFilter>(paginationQuery);

        var issueFields = await _unitofWork.IssueFieldRepository.GetAll(filter, cts);
        await _unitofWork.CommitAsync(cts);

        var result = _mapper.Map<IEnumerable<IssueField>>(issueFields);
        return result;
    }
}