﻿using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IIssueTypeRepository
{
    Task<IEnumerable<IssueTypeModel>> GetAll(
        CancellationToken cts,
        PaginationFilter? filter = null
    );
    Task<IssueTypeModel?> Get(int issueTypeId, CancellationToken cts);
    Task<IssueTypeModel> Add(IssueTypeModel issueType, CancellationToken cts);
    Task<IssueTypeModel> ChangeName(IssueTypeModel issueType, CancellationToken cts);
    Task<IssueTypeModel> Delete(int issueTypeId, CancellationToken cts);
    Task<bool> DoesExist(int issueTypeId, CancellationToken cts);
    Task<bool> DoesExist(string issueTypeName, CancellationToken cts);
    Task<int> Count(CancellationToken cts);
}