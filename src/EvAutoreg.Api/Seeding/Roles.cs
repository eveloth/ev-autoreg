using EvAutoreg.Data.Models;

namespace EvAutoreg.Api.Seeding;

public class Roles
{
    public static RoleModel DefaultRole { get; } = new()
    {
        RoleName = "superadmin"
    };
}