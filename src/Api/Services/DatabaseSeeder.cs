using Api.Contracts.Dto;
using Api.Services.Interfaces;
using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using MapsterMapper;
using Npgsql;

namespace Api.Services;

public class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitofWork _unitofWork;

    public DatabaseSeeder(
        ILogger<DatabaseSeeder> logger,
        IMapper mapper,
        IPasswordHasher hasher,
        IUnitofWork unitofWork
    )
    {
        _logger = logger;
        _mapper = mapper;
        _hasher = hasher;
        _unitofWork = unitofWork;
    }

    public async Task SeedData()
    {
        _logger.LogInformation("Initialiizing database seeding...");

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            _logger.LogInformation("Seeding issue fields...");

            var issueFieldTableIsEmpty = await _unitofWork.GpRepository.IsTableEmpty(
                "issue_field",
                ct
            );

            if (issueFieldTableIsEmpty)
            {
                await SeedIssueFields(ct);
                await _unitofWork.CommitAsync(ct);

                _logger.LogInformation("Finished seeding issue fields");
            }
            else
            {
                _logger.LogInformation("Issue fields table OK, skipping seeding...");
            }

            _logger.LogInformation("Seeding permissions...");

            var permissionTableIsEmply = await _unitofWork.GpRepository.IsTableEmpty(
                "permission",
                ct
            );

            if (permissionTableIsEmply)
            {
                await SeedPermissions(ct);
                await _unitofWork.CommitAsync(ct);

                _logger.LogInformation("Finished seeding permissions");
            }
            else
            {
                _logger.LogInformation("Permissions table OK, skipping seeding...");
            }

            _logger.LogInformation("Seeding default user and role...");

            var roleTableIsEmply = await _unitofWork.GpRepository.IsTableEmpty("role", ct);
            var userTableIsEmply = await _unitofWork.GpRepository.IsTableEmpty("app_user", ct);
            var rolePermissionTableIsEmply = await _unitofWork.GpRepository.IsTableEmpty(
                "role_permission",
                ct
            );

            if (userTableIsEmply || roleTableIsEmply || rolePermissionTableIsEmply)
            {
                _logger.LogInformation(
                    "Roles, user or role-permission tables are empty. " +
                    "Seeding data to into all of them to ensure proper behaviour..."
                );

                var role = await SeedDefaultRole(ct);
                await AddPermissionsToDefaultRole(role.Id, ct);
                await SeedDefaultUser(role.Id, ct);

                await _unitofWork.CommitAsync(ct);
            }
            else
            {
                _logger.LogInformation("Tables are OK");
            }


            _logger.LogInformation("Finished seeding data");
        }
        catch (NpgsqlException e)
        {
            _logger.LogCritical(
                "An error occured while seeding permissions: {ErrorMessage}",
                e.Message
            );

            throw;
        }
        catch (Exception e)
        {
            _logger.LogCritical("Unknown error occured while seeding data: {ErrorMessage}", e);
            throw;
        }
    }

    private async Task SeedIssueFields(CancellationToken ct)
    {
        var issueFields = new[]
        {
            "Author",
            "Company",
            "Priority",
            "ShortDescription",
            "Description"
        };

        foreach (var field in issueFields)
        {
            await _unitofWork.IssueFieldRepository.Add(field, ct);
        }
    }

    private async Task SeedPermissions(CancellationToken ct)
    {
        var permissions = Permissions.GetPermissions();
        foreach (var permission in permissions)
        {
            await _unitofWork.PermissionRepository.Add(permission, ct);
        }
    }

    private async Task<RoleDto> SeedDefaultRole(CancellationToken ct)
    {
        const string defaultRoleName = "superadmin";

        return _mapper.Map<RoleDto>(await _unitofWork.RoleRepository.Add(defaultRoleName, ct));
    }

    private async Task AddPermissionsToDefaultRole(int roleId, CancellationToken ct)
    {
        var paginationDummy = new PaginationFilter(1, 100000);
        var permissions = await _unitofWork.PermissionRepository.GetAll(
            paginationDummy,
            ct
        );

        foreach (var permission in permissions)
        {
            await _unitofWork.RolePermissionRepository.AddPermissionToRole(
                roleId,
                permission.Id,
                ct
            );
        }
    }

    private async Task SeedDefaultUser(int roleId, CancellationToken ct)
    {
        const string password = "P@assw0rd123";

        var passwordHash = _hasher.HashPassword(password);

        var user = new UserModel { Email = "eadmin@vautoreg.org", PasswordHash = passwordHash };

        var defaultUser = await _unitofWork.UserRepository.Create(user, ct);
        await _unitofWork.RoleRepository.SetUserRole(defaultUser.Id, roleId, ct);
    }
}