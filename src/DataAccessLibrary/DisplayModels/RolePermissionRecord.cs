using DataAccessLibrary.DbModels;

namespace DataAccessLibrary.DisplayModels;

public class RolePermissionRecord
{
    public Role Role { get; set; }
    public List<Permission> Permissions { get; set; } = new();
}
