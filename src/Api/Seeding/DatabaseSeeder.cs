using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using Npgsql;

namespace Api.Seeding;

public class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitofWork _unitofWork;

    public DatabaseSeeder(
        ILogger<DatabaseSeeder> logger,
        IPasswordHasher hasher,
        IUnitofWork unitofWork
    )
    {
        _logger = logger;
        _hasher = hasher;
        _unitofWork = unitofWork;
    }

    public async Task SeedData()
    {
        var ct = CancellationToken.None;
        _logger.LogInformation("Initializing database seeding");

        try
        {
            if (await AreTablesEmpty())
            {
                _logger.LogInformation("Performing initial seeding");
                await PerformInitialSeeding();
            }
            else
            {
                _logger.LogInformation("Tables are OK, skipping initial seeding");
            }

            await EnsurePermissionsUpToDate();
            await EnsureIssueFieldsUpToDate();
            await _unitofWork.CommitAsync(ct);

            _logger.LogInformation("Database seeding completed");
        }
        catch (NpgsqlException e)
        {
            _logger.LogCritical(
                "Failed to seed data, error talking to the database: {Exception}",
                e
            );
            throw;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Failed to seed data, unknown error: {Exception}", e);
            throw;
        }
    }

    private async Task<bool> AreTablesEmpty()
    {
        var isEmpty = _unitofWork.GpRepository.IsTableEmpty;
        var ct = CancellationToken.None;

        var isUserTableEmpty = await isEmpty("app_user", ct);
        var isRoleTableEmpty = await isEmpty("role", ct);
        var isPermissionTableEmpty = await isEmpty("permission", ct);
        var isRolePermissionTableEmpty = await isEmpty("role_permission", ct);
        var isIssueFieldTableEmpty = await isEmpty("issue_field", ct);

        return isUserTableEmpty
            && isRoleTableEmpty
            && isPermissionTableEmpty
            && isRolePermissionTableEmpty
            && isIssueFieldTableEmpty;
    }

    private async Task PerformInitialSeeding()
    {
        var ct = CancellationToken.None;

        var permissions = Permissions.GetPermissions();
        var insertedPermissions = new List<PermissionModel>();

        foreach (var permission in permissions)
        {
            insertedPermissions.Add(await _unitofWork.PermissionRepository.Add(permission, ct));
        }

        var role = Roles.DefaultRole;
        var insertedRole = await _unitofWork.RoleRepository.Add(role, ct);

        var rolePermissions = insertedPermissions
            .Select(x => new RolePermissionModel { RoleId = insertedRole.Id, PermissionId = x.Id })
            .ToList();

        foreach (var rolePermission in rolePermissions)
        {
            await _unitofWork.RolePermissionRepository.AddPermissionToRole(rolePermission, ct);
        }

        var user = CreateDefaultUser();
        var insertedUser = await _unitofWork.UserRepository.Create(user, ct);
        insertedUser.RoleId = insertedRole.Id;
        await _unitofWork.UserRepository.AddUserToRole(insertedUser, ct);

        var issueFields = IssueFields.DefaultIssueFileds;

        foreach (var issueField in issueFields)
        {
            await _unitofWork.IssueFieldRepository.Add(issueField, ct);
        }
    }

    private async Task EnsurePermissionsUpToDate()
    {
        _logger.LogInformation("Ensuring permissions are up to date...");

        var ct = CancellationToken.None;
        var paginationDummy = new PaginationFilter(1, 100000);

        var createdPermissions = (
            await _unitofWork.PermissionRepository.GetAll(paginationDummy, ct)
        ).ToList();
        var availablePermissions = Permissions.GetPermissions();

        var lackingDiff = availablePermissions
            .ExceptBy(createdPermissions.Select(x => x.PermissionName), y => y.PermissionName)
            .ToList();

        var excessiveDiff = createdPermissions
            .ExceptBy(availablePermissions.Select(x => x.PermissionName), y => y.PermissionName)
            .ToList();

        if (!lackingDiff.Any() && !excessiveDiff.Any())
        {
            _logger.LogInformation("Permissions are up to date");
            return;
        }

        foreach (var permission in lackingDiff)
        {
            await _unitofWork.PermissionRepository.Add(permission, ct);
        }

        foreach (var permission in excessiveDiff)
        {
            await _unitofWork.PermissionRepository.Delete(permission.Id, ct);
        }

        _logger.LogInformation("Permissions were successfully updated");
    }

    private async Task EnsureIssueFieldsUpToDate()
    {
        _logger.LogInformation("Ensuring issue fields are up to date...");

        var ct = CancellationToken.None;
        var paginationDummy = new PaginationFilter(1, 100000);

        var createdIssueFiels = (
            await _unitofWork.IssueFieldRepository.GetAll(paginationDummy, ct)
        ).ToList();
        var availableIssueFields = IssueFields.DefaultIssueFileds;

        var lackingDiff = availableIssueFields
            .ExceptBy(createdIssueFiels.Select(x => x.FieldName), y => y.FieldName)
            .ToList();

        var excessiveDiff = createdIssueFiels
            .ExceptBy(availableIssueFields.Select(x => x.FieldName), y => y.FieldName)
            .ToList();

        if (!lackingDiff.Any() && !excessiveDiff.Any())
        {
            _logger.LogInformation("Issue fields are up to date");
            return;
        }

        foreach (var issueField in lackingDiff)
        {
            await _unitofWork.IssueFieldRepository.Add(issueField, ct);
        }

        foreach (var issueField in excessiveDiff)
        {
            await _unitofWork.IssueFieldRepository.Delete(issueField.Id, ct);
        }

        _logger.LogInformation("Issue fields were successfully updated");
    }

    private UserModel CreateDefaultUser()
    {
        var passwordHash = _hasher.HashPassword(Users.DefaultPassword);

        return new UserModel { Email = Users.DefaultEmail, PasswordHash = passwordHash };
    }
}