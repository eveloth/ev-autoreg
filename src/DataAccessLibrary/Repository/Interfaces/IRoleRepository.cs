﻿using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;

namespace DataAccessLibrary.Repository.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<RoleModel>> GetAll(PaginationFilter filter, CancellationToken cts);
    Task<RoleModel> Add(string roleName, CancellationToken cts);
    Task<RoleModel> ChangeName(RoleModel role, CancellationToken cts);
    Task<RoleModel> Delete(int roleId, CancellationToken cts);
    Task<bool> DoesExist(int roleId, CancellationToken cts);
    Task<bool> DoesExist(string roleName, CancellationToken cts);
    Task<UserProfileModel> SetUserRole(int userId, int roleId, CancellationToken cts);
    Task<UserProfileModel> RemoveUserFromRole(int userId, CancellationToken cts);
}