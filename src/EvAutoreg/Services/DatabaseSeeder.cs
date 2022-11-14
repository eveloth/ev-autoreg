using System.Data;
using System.Data.Common;
using DataAccessLibrary.DbModels;
using DataAccessLibrary.Repositories;
using Npgsql;

namespace EvAutoreg.Services;

public class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly DbTransaction _transaction;
    private readonly IGeneralPurposeRepository _gpRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccessControlRepository _acRepository;
    private readonly IPasswordHasher _hasher;

    public DatabaseSeeder(
        ILogger<DatabaseSeeder> logger,
        IUserRepository userRepository,
        IAccessControlRepository acRepository,
        IPasswordHasher hasher,
        IGeneralPurposeRepository gpRepository,
        DbTransaction transaction
    )
    {
        _logger = logger;
        _userRepository = userRepository;
        _acRepository = acRepository;
        _hasher = hasher;
        _gpRepository = gpRepository;
        _transaction = transaction;
    }

    public async Task SeedData()
    {
        _logger.LogInformation("Initialiizing database seeding...");

        _logger.LogInformation("Seeding permissions...");

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            var permissionTableIsNotEmply = await _gpRepository.IsTableEmpty("permission", ct);
            /*
            var userTableIsEmply = await _gpRepository.IsTableEmpty("app_user", ct);
            var roleTableIsEmply = await _gpRepository.IsTableEmpty("role", ct);
            var rolePermissionTableIsEmply = await _gpRepository.IsTableEmpty(
                "role_permission",
                ct
            );
            */

            if (
                permissionTableIsNotEmply
            )
            {
                _logger.LogInformation("Tables OK, skipping seeding");
                return;
            }

            await SeedPermissions(ct);
            /*
            await SeedDefaultRole(ct);
            await AddPermissionsToDefaultRole(ct);
            await SeedDefaultUser(ct);
            */


            _logger.LogInformation("Finished seeding permissions");
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
            _logger.LogCritical("Unknown error: {ErrorMessage}", e.Message);
            _logger.LogCritical("{Error}", e);
            throw;
        }

    }

    private async Task SeedPermissions(CancellationToken ct)
    {
            var permissions = Permissions.GetPermissions();
            var permission = permissions.First();
            Console.WriteLine(permission.Id.Value + " " + permission.PermissionName + " " + permission.Description);
            await _acRepository.AddPermission(permission, ct);
    }
    private async Task SeedDefaultRole(CancellationToken ct)
    {
        const string defaultRoleName = "superadmin";

        await _acRepository.AddRole(defaultRoleName, ct);
    }

    private async Task AddPermissionsToDefaultRole(CancellationToken ct)
    {
        var permissions = await _acRepository.GetAllPermissions(ct);

        foreach (var permission in permissions)
        {
            await _acRepository.AddPermissionToRole(1, permission.Id.Value, ct);
        }
    }

    private async Task SeedDefaultUser(CancellationToken ct)
    {
        const string password = "P@assw0rd123";

        var passwordHash = _hasher.HashPassword(password);

        var user = new UserModel
        {
            Email = "admin@evautoreg.org",
            PasswordHash = passwordHash,
            FirstName = "Vadim",
            LastName = "Komissarov",
            IsBlocked = false,
            IsDeleted = false,
            RoleId = 1
        };

        await _userRepository.CreateUser(user, ct);
    }
}