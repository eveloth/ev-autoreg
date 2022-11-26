using DataAccessLibrary.Filters;
using DataAccessLibrary.Models;
using DataAccessLibrary.Repository.Interfaces;
using EvAutoreg.Contracts.Dto;
using EvAutoreg.Services.Interfaces;
using MapsterMapper;
using Npgsql;

namespace EvAutoreg.Services;

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

        _logger.LogInformation("Seeding permissions...");

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            var permissionTableIsNotEmply = await _unitofWork.GpRepository.IsTableEmpty(
                "permission",
                ct
            );
            var userTableIsNotEmply = await _unitofWork.GpRepository.IsTableEmpty("app_user", ct);
            var roleTableIsNotEmply = await _unitofWork.GpRepository.IsTableEmpty("role", ct);
            var rolePermissionTableIsNotEmply = await _unitofWork.GpRepository.IsTableEmpty(
                "role_permission",
                ct
            );

            if (
                permissionTableIsNotEmply
                || userTableIsNotEmply
                || roleTableIsNotEmply
                || rolePermissionTableIsNotEmply
            )
            {
                _logger.LogInformation("Tables OK, skipping seeding");
                return;
            }

            await SeedPermissions(ct);
            var role = await SeedDefaultRole(ct);
            await AddPermissionsToDefaultRole(role.Id, ct);
            await SeedDefaultUser(role.Id, ct);

            await _unitofWork.CommitAsync(ct);

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
        foreach (var permission in permissions)
        {
            await _unitofWork.PermissionRepository.AddPermission(permission, ct);
        }
    }

    private async Task<RoleDto> SeedDefaultRole(CancellationToken ct)
    {
        const string defaultRoleName = "superadmin";

        return _mapper.Map<RoleDto>(await _unitofWork.RoleRepository.AddRole(defaultRoleName, ct));
    }

    private async Task AddPermissionsToDefaultRole(int roleId, CancellationToken ct)
    {
        var paginationDummy = new PaginationFilter(1, 100000);
        var permissions = await _unitofWork.PermissionRepository.GetAllPermissions(
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

        var defaultUser = await _unitofWork.UserRepository.CreateUser(user, ct);
        await _unitofWork.RoleRepository.SetUserRole(defaultUser.Id, roleId, ct);
    }
}
