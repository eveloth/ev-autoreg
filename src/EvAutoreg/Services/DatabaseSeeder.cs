using DataAccessLibrary.Repositories;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Npgsql;

namespace EvAutoreg.Services;

public class DatabaseSeeder
{
    private readonly ILogger<DatabaseSeeder> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAccessControlRepository _acRepository;
    private readonly IPasswordHasher _hasher;

    public DatabaseSeeder(ILogger<DatabaseSeeder> logger, IUserRepository userRepository, IAccessControlRepository acRepository, IPasswordHasher hasher)
    {
        _logger = logger;
        _userRepository = userRepository;
        _acRepository = acRepository;
        _hasher = hasher;
    }

    public async Task SeedDefaultRolesAndPermissions()
    {
        _logger.LogInformation("Initialiizing database seeding...");
        
        _logger.LogInformation("Seeding permissions...");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var ct = cts.Token;

        try
        {
            var affectedRows = await _acRepository.ClearPermissions(ct);
            
            _logger.LogInformation("Rows affected: {RowCount}", affectedRows);

            var permissions = Permissions.GetPermissions();
            
            foreach (var permission in permissions)
            {
               await _acRepository.AddPermission(permission, ct);
            }
            
            _logger.LogInformation("Finished seeding permissions");
        }
        catch (NpgsqlException e)
        {
            _logger.LogCritical("An error occured while seeding permissions: {ErrorMessage}", e.Message);
            throw;
        }

        var defaultRoleName = "superadmin";
    }

    public async Task SeedDefaultUser()
    {
        await Task.CompletedTask;
    }

}