﻿using EvAutoreg.Data.Filters;
using EvAutoreg.Data.Models;

namespace EvAutoreg.Data.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<PermissionModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<PermissionModel?> Get(int permissionId, CancellationToken cts);
    Task<PermissionModel> Add(PermissionModel permission, CancellationToken cts);
    Task<PermissionModel> Delete(int permissionId, CancellationToken cts);
    Task<int> Clear(CancellationToken cts);
    Task<bool> DoesExist(int permissionId, CancellationToken cts);
    Task<bool> DoesExist(string permissionName, CancellationToken cts);
}