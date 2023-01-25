using DataAccessLibrary.Models;

namespace Api.Seeding;

public class Roles
{
    public static RoleModel DefaultRole { get; } = new()
    {
        RoleName = "superadmin"
    };
}