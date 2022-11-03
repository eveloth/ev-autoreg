﻿using Dapper;
using DataAccessLibrary.Models;
using DataAccessLibrary.SqlDataAccess;

namespace DataAccessLibrary.Repositories;

public class UserRepository : IUserRepository 
{
    private readonly ISqlDataAccess _db;
    
    public UserRepository(ISqlDataAccess db)
    {
        _db = db;
    }

    public async Task<bool> DoesUserExist(int id, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE id = @Id)";

        return await _db.LoadFirst<bool, object>(sql, new { Id = id }, cts);
    }

    public async Task<bool> DoesUserExist(string email, CancellationToken cts)
    {
        const string sql = @"SELECT EXISTS (SELECT true FROM app_user WHERE email = @Email)";

        return await _db.LoadFirst<bool, object>(sql, new { email }, cts);
    }
    
    public async Task<IEnumerable<UserModel>> GetAllUsers(CancellationToken cts, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true => @"SELECT * FROM app_user",
            false => @"SELECT * FROM app_user WHERE is_deleted = false"
        };

        return await _db.LoadAllData<UserModel>(sql, cts);
    }

    public async Task<UserModel?> GetUserById(int id, CancellationToken cts, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true  => @"SELECT * FROM app_user WHERE id = @Id",
            false => @"SELECT * FROM app_user WHERE id = @Id and is_deleted = false"
        };

        return await _db.LoadFirst<UserModel, object>(sql, new { Id = id }, cts);
    }
    
    public async Task<UserModel?> GetUserByEmail(string email, CancellationToken cts, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true  => @"SELECT * FROM app_user WHERE email = @Email",
            false => @"SELECT * FROM app_user WHERE email = @Email and is_deleted = false"
        };
        
        // В отличие от ситуции, когда в качестве параметра передаётся int, просто так передать string в качестве
        // параметра у меня не вышло; operator does not exist: @ character varying
        return await _db.LoadFirst<UserModel, object>(sql, new { email }, cts);
    }
    
    public async Task<UserProfile> CreateUser(UserModel user, CancellationToken cts)
    {
        var parameters = new DynamicParameters(user);
        const string sql = @"INSERT INTO app_user (email, password_hash, first_name, last_name, is_blocked, is_deleted)
                             VALUES (@Email, @PasswordHash, @FirstName, @LastName, @IsBlocked, @IsDeleted)
                             RETURNING id, email, first_name, last_name, is_blocked, is_deleted";

        return await _db.SaveData<object, UserProfile>(sql, parameters, cts);
    }

    public async Task<IEnumerable<UserProfile>> GetAllUserProfiles(CancellationToken cts, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true  => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user",
            false  => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user WHERE is_deleted = false"
        };

        return await _db.LoadAllData<UserProfile>(sql, cts);
    }

    public async Task<UserProfile?> GetUserProfle(int id, CancellationToken cts, bool includeDeleted = false)
    {
        var sql = includeDeleted switch
        {
            true  => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user WHERE id = @Id",
            false  => @"SELECT id, email, first_name, last_name, is_deleted, is_blocked FROM app_user WHERE id = @Id and is_deleted = false"
        };

        return await _db.LoadFirst<UserProfile?, object>(sql, new {Id = id}, cts);
    }

    public async Task<int> UpdateUserPassword(int id, string passwordHash, CancellationToken cts)
    {
        var parameters = new DynamicParameters(new {Id = id, PasswordHash = passwordHash});
        const string sql = @"UPDATE app_user SET password_hash = @PasswordHash WHERE id = @Id RETURNING id";

        return await _db.SaveData<object, int>(sql, parameters, cts);
    }

    public async Task<UserProfile> UpdateUserEmail(int id, string newEmail, CancellationToken cts)
    {
        var parameters = new DynamicParameters(new { Id = id, Email = newEmail});
        const string sql = @"UPDATE app_user SET email = @Email WHERE id = @Id
                             RETURNING id, email, first_name, last_name, is_deleted, is_blocked";

        return await _db.SaveData<object, UserProfile>(sql, parameters, cts);
    }

    public async Task<UserProfile> UpdateUserProfile(int id, string firstName, string lastName, CancellationToken cts)
    {
        var parameters = new DynamicParameters(new {Id = id, FirstName = firstName, LastName = lastName});
        const string sql = @"UPDATE app_user SET first_name = @FirstName, last_name = @LastName WHERE id = @Id
                             RETURNING id, email, first_name, last_name, is_deleted, is_blocked";

        return await _db.SaveData<object, UserProfile>(sql, parameters, cts);
    }
    
    public async Task<int> BlockUser(int id, CancellationToken cts)
    {
        const string sql = @"UPDATE app_user SET is_blocked = true WHERE id = @Id RETURNING id";

        return await _db.SaveData<object, int>(sql, new { Id = id}, cts);
    }

    public async Task<int> UnblockUser(int id, CancellationToken cts)
    {
        const string sql = @"UPDATE app_user SET is_blocked = false WHERE id = @Id RETURNING id";

        return await _db.SaveData<object, int>(sql, new {Id = id}, cts);
    }
    
    public async Task<UserProfile> DeleteUser(int id, CancellationToken cts)
    {
        const string sql = @"UPDATE app_user SET is_deleted = true WHERE id = @Id
                             RETURNING id, email, first_name, last_name, is_deleted, is_blocked";

        return await _db.SaveData<object, UserProfile>(sql, new {Id = id}, cts);
    }
}